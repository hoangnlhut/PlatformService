using Microsoft.Extensions.Options;
using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly RabbitMqConnectionProvider _rabbitMqConnectionProvider;
        private IChannel? _channel;
        private const string exchangeName = "publish_new_platform_fanout_trigger";

        public MessageBusClient(RabbitMqConnectionProvider rabbitMqConnectionProvider)
        {
            _rabbitMqConnectionProvider = rabbitMqConnectionProvider;
        }

        private async Task<IChannel> GetChannelAsync()
        {
            if (_channel is { IsOpen: true })
                return _channel;

            var connection = await _rabbitMqConnectionProvider.GetConnectionAsync();

            //var options = new CreateChannelOptions(
            //        publisherConfirmationsEnabled: true,
            //        publisherConfirmationTrackingEnabled: true
            //    );

            // add options in CreateChannelAsync if needed

            _channel = await connection.CreateChannelAsync();

            return _channel;
        }
        public async void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            if(_channel is null || !_channel.IsOpen)
            {
                _channel = await GetChannelAsync();
            }

            Console.WriteLine("--> RabbitMQ connection open and channel created");

            // Declare the Fanout Exchange
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            // 2. Prepare the payload
            var messageJson = JsonSerializer.Serialize(platformPublishedDto);
            var body = Encoding.UTF8.GetBytes(messageJson);

            //3. Publish to the fanout exchange (routingKey is ignored)
            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: "",
                body: body);

            Console.WriteLine($"[PlatformService] Broadcasted: {messageJson}");

            //await DisposeChannelAsync();
            //Console.WriteLine($"[PlatformService] Disposed channel successfully");
        }


        private async ValueTask DisposeAsync()
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
        }
    }



}
