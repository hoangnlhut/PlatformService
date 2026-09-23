using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public sealed class RabbitMqConnectionProvider: IAsyncDisposable
    {
        private IConnection? _connection;
        private readonly IConfiguration _configuration;

        public RabbitMqConnectionProvider(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IConnection> GetConnectionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"] ?? throw new ArgumentNullException("RabbitMQHost"),
                Port = int.Parse(_configuration["RabbitMQPort"] ?? throw new ArgumentNullException("RabbitMQPort")),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                NetworkRecoveryInterval =TimeSpan.FromSeconds(5)
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);

            Console.WriteLine("--> RabbitMQ connection established.");

            return _connection;
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }

}
