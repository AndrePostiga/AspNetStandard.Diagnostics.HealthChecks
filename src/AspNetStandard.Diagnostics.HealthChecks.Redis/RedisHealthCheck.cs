using AspNetStandard.Diagnostics.HealthChecks.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.Redis
{
    public class RedisHealthCheck : IHealthCheck
    {
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private readonly string _redisConnectionString;

        public RedisHealthCheck(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            ConnectionMultiplexer connection = null;
            try
            {
                if (!_connections.TryGetValue(_redisConnectionString, out connection))
                {
                    connection = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                    if (!_connections.TryAdd(_redisConnectionString, connection))
                    {
                        connection.Dispose();
                        connection = _connections[_redisConnectionString]; 
                    }

                }

                await connection?.GetDatabase().PingAsync();
                var result = new HealthCheckResult(HealthStatus.Healthy, "Redis is healthy");

                connection?.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "Redis is unhealthy", exception: ex);
            }
            finally
            {
                connection?.Dispose();
            }
        }
    }
}
