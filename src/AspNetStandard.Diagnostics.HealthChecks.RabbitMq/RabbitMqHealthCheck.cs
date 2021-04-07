using AspNetStandard.Diagnostics.HealthChecks.Entities;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.RabbitMq
{
    public class RabbitMqHealthCheck : IHealthCheck, IDisposable // ToDo: Caso não seja estático, a conexão deve ser encerrada por conta de leak
    {
        private bool _disposed; // ToDo: Implementei a interface IDisposable
        private static volatile object _sync = new object(); // ToDo: Fiz uma implementação de multithread
        private IConnection _connection; // ToDo: Deveria ser estático? Um pool?
        private readonly IConnectionFactory _factory;

        public RabbitMqHealthCheck(IConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public RabbitMqHealthCheck(IConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public RabbitMqHealthCheck(Uri rabbitConnectionString, SslOption ssl = null) : this(CreateFactory(rabbitConnectionString, ssl))
        {
        }

        public RabbitMqHealthCheck(string rabbitConnectionString, SslOption ssl = null) : this(
            new Uri(rabbitConnectionString), ssl)
        {
        }

        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                EnsureConnection();

                using (_connection.CreateModel())
                {
                    return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "RabbitMQ is healthy"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "RabbitMQ is unhealthy", exception: ex));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void EnsureConnection()
        {
            if (_connection != null) return;

            lock (_sync)
            {
                if (_connection != null) return;
                _connection = _factory.CreateConnection();
            }
        }

        ~RabbitMqHealthCheck()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                _connection?.Close();
                _connection?.Dispose();
            }

            _disposed = true;
        }

        private static ConnectionFactory CreateFactory(Uri connectionString, SslOption sslOption)
        {
            return new ConnectionFactory()
            {
                Uri = connectionString,
                AutomaticRecoveryEnabled = true,
                UseBackgroundThreadsForIO = true,
                Ssl = sslOption ?? new SslOption()
            };
        }
    }
}