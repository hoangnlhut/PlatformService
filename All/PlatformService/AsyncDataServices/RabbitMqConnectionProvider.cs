using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public sealed class RabbitMqConnectionProvider
    {
        private IConnection? _connection;
        private readonly IConfiguration _configuration;

        public RabbitMqConnectionProvider(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IConnection> GetConnectionAsync()
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
            Console.WriteLine($"--> RabbitMQ connection factory created. Host: {factory.HostName}. Port: {factory.Port}");

            _connection = await factory.CreateConnectionAsync();
            Console.WriteLine("--> RabbitMQ connection established.");

            return _connection;
        }
    }

}
