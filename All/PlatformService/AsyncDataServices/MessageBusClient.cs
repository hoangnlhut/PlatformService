using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly RabbitMqConnectionProvider _rabbitMqConnectionProvider;
        private IChannel? _channel;
        private IConnection? _connection;
        private const string exchangeName = "publish_new_platform_fanout_trigger";

        public MessageBusClient(RabbitMqConnectionProvider rabbitMqConnectionProvider)
        {
            _rabbitMqConnectionProvider = rabbitMqConnectionProvider;
        }

        private async Task<IChannel> GetChannelAsync()
        {
            if (_channel is { IsOpen: true })
                return _channel;

            _connection = await _rabbitMqConnectionProvider.GetConnectionAsync();

            //var options = new CreateChannelOptions(
            //        publisherConfirmationsEnabled: true,
            //        publisherConfirmationTrackingEnabled: true
            //    );

            // add options in CreateChannelAsync if needed

            _channel = await _connection.CreateChannelAsync();

            Console.WriteLine("--> RabbitMQ Channel created successfully.");

            return _channel;
        }
        public async void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            if (_channel is null || !_channel.IsOpen)
            {
                _channel = await GetChannelAsync();
            }

            // Declare the Fanout Exchange
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout
            //durable: true,
            //autoDelete: false,
            //arguments: null
            );

            _connection!.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

            // 2. Prepare the payload
            Console.WriteLine($"[PlatformService] Sending message......");

            var messageJson = JsonSerializer.Serialize(platformPublishedDto);
            var body = Encoding.UTF8.GetBytes(messageJson);

            //3. Publish to the fanout exchange (routingKey is ignored)
            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: "",
                //mandatory: false,
                //basicProperties: null,
                body: body);

            Console.WriteLine($"[PlatformService] Broadcasted: {messageJson}");

            //await DisposeChannelAsync();
            //Console.WriteLine($"[PlatformService] Disposed channel successfully");
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine($"[PlatformService] RabbitMQ connection shutdown");
        }

        public async void Dispose()
        {
            if (_channel is { IsOpen: true })
            {
                await _channel.CloseAsync();
                await _connection!.CloseAsync();
                await _channel.DisposeAsync();
                await _connection!.DisposeAsync();
            }

            Console.WriteLine($"[PlatformService] Disposing channel and connection.");
        }
    }
}
