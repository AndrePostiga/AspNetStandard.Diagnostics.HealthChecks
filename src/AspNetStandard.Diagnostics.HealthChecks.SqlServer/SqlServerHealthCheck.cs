using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.SqlServer
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _sql;
        private readonly Action<SqlConnection> _beforeOpenConnectionConfigurer;

        public SqlServerHealthCheck(string sqlServerConnectionString, string sql, Action<SqlConnection> beforeOpenConnectionConfigurer = null)
        {
            _connectionString = sqlServerConnectionString ?? throw new ArgumentNullException(nameof(sqlServerConnectionString));
            _sql = sql ?? throw new ArgumentNullException(nameof(sql));
            _beforeOpenConnectionConfigurer = beforeOpenConnectionConfigurer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    _beforeOpenConnectionConfigurer?.Invoke(connection);

                    await connection.OpenAsync(cancellationToken);
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _sql;
                        await command.ExecuteScalarAsync(cancellationToken);
                    }
                    return new HealthCheckResult(HealthStatus.Healthy, "SqlServer is healthy");
                }
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "SqlServer is unhealthy", ex);
            }
        }
    }
}
