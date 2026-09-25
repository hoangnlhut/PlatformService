using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string exchangeName = "publish_new_platform_fanout_trigger";
        private string _queueName = string.Empty;

        public MessageBusSubscriber(IEventProcessor eventProcessor, IConfiguration configuration)
        {
            _eventProcessor = eventProcessor;
            _configuration = configuration;

            InitializeMessageBus().GetAwaiter().GetResult();
        }

        private async Task InitializeMessageBus()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"] ?? throw new ArgumentNullException("RabbitMQ:HostName"),
                Port = int.Parse(_configuration["RabbitMQPort"] ?? throw new ArgumentNullException("RabbitMQ:Port"))
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

            // declare a server-named queue
            QueueDeclareOk queueDeclareResult = await _channel!.QueueDeclareAsync();
            _queueName = queueDeclareResult.QueueName;

            Console.WriteLine($"--> Queue declared in Command service: {_queueName}");

            await _channel.QueueBindAsync(queue: _queueName, exchange: exchangeName, routingKey: string.Empty);

            Console.WriteLine($"--> Listening on the message bus..");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("--> RabbitMQ connection is shutting down.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if(_channel is null) {
                throw new InvalidOperationException("RabbitMQ channel is not initialized.");
            }

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Process the message using the event processor
                _eventProcessor.ProcessEvent(message);
                return Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);
        }

        public override async void Dispose()
        {
            if (_channel is { IsOpen: true })
            {
                await _channel.CloseAsync();
                await _connection!.CloseAsync();
                await _channel.DisposeAsync();
                await _connection!.DisposeAsync();
            }

            base.Dispose();
            Console.WriteLine("--> Disposing MessageBusSubscriber.");
        }
    }
}
