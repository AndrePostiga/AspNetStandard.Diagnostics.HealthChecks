using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Errors;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("AspNetStandard.Diagnostics.HealthChecks.Tests")]

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal class HealthCheckService : IHealthCheckService
    {
        private HealthChecksBuilder _healthChecksBuilder { get; }

        public HealthCheckService(HealthChecksBuilder healthChecksBuilder)
        {
            _healthChecksBuilder = healthChecksBuilder;
        }

        public async Task<HealthCheckResponse> GetHealthAsync(CancellationToken cancellationToken = default)
        {
            var healthCheckResponse = new HealthCheckResponse();
            var sw = new Stopwatch();

            foreach (var task in _healthChecksBuilder.HealthChecks)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    sw.Reset();
                    sw.Start();
                    var result = await task.Value.CheckHealthAsync(cancellationToken);
                    sw.Stop();

                    healthCheckResponse.Entries.Add(task.Key, new HealthCheckResultExtended(result) { ResponseTime = sw.ElapsedMilliseconds });
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    healthCheckResponse.Entries.Add(task.Key, new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)));
                }
            }

            return healthCheckResponse;
        }

        public async Task<HealthCheckResultExtended> GetHealthAsync(string healthCheckName, CancellationToken cancellationToken = default)
        {
            if (!_healthChecksBuilder.HealthChecks.TryGetValue(healthCheckName, out var healthCheck))
            {
                throw new NotFoundError(healthCheckName);
            }

            try
            {
                var sw = new Stopwatch();
                sw.Reset();
                sw.Start();
                var result = await healthCheck.CheckHealthAsync(cancellationToken);
                sw.Stop();
                return new HealthCheckResultExtended(result) { ResponseTime = sw.ElapsedMilliseconds };
            }
            catch
            {
                return new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy));
            }
        }

        public HttpStatusCode GetStatusCode(HealthStatus healthstatus)
        {
            return _healthChecksBuilder.ResultStatusCodes[healthstatus];
        }
    }
}