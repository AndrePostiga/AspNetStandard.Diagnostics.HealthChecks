using AspNetStandard.Diagnostics.HealthChecks.Entities;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.RabbitMq
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private IConnection _connection;
        private IConnectionFactory _factory;
        private readonly Uri _rabbitConnectionString;
        private readonly SslOption _sslOption;

        #region ctor

        public RabbitMqHealthCheck(IConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public RabbitMqHealthCheck(IConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public RabbitMqHealthCheck(Uri rabbitConnectionString, SslOption ssl)
        {
            _rabbitConnectionString = rabbitConnectionString;
            _sslOption = ssl;
        }

        #endregion ctor

        #region Interface Implementation

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

        private void EnsureConnection()
        {
            if (_connection == null)
            {
                if (_factory == null)
                {
                    _factory = new ConnectionFactory()
                    {
                        Uri = _rabbitConnectionString,
                        AutomaticRecoveryEnabled = true,
                        UseBackgroundThreadsForIO = true,
                        Ssl = _sslOption ?? new SslOption()
                    };
                }

                _connection = _factory.CreateConnection();
            }
        }

        #endregion Interface Implementation
    }
}