using AspNetStandard.Diagnostics.HealthChecks.Entities;
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

        //public async Task<HealthCheckResponse> GetHealthAsync(CancellationToken cancellationToken = default)
        //{
        //    var healthCheckResponse = new HealthCheckResponse();
        //    var tasks = _healthChecksInstances.Select(c => new { name = c.Key, result = c.Value.CheckHealthAsync(cancellationToken) });
        //    var sw = new Stopwatch();
        //    await Task.Run(() =>
        //    {
        //        Parallel.ForEach(tasks, async task =>
        //        {
        //            try
        //            {
        //                cancellationToken.ThrowIfCancellationRequested();
        //                sw.Reset();
        //                sw.Start();
        //                var result = await task.result;
        //                sw.Stop();
        //                healthCheckResponse.Entries.Add(task.name, new HealthCheckResultExtended(result) { ResponseTime = sw.ElapsedMilliseconds });
        //            }
        //            catch (OperationCanceledException)
        //            {
        //                throw;
        //            }
        //            catch
        //            {
        //                healthCheckResponse.Entries.Add(task.name, new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)));
        //            }
        //        });
        //    });

        //    healthCheckResponse.SetOverAllStatus();
        //    healthCheckResponse.TotalResponseTime = healthCheckResponse.Entries.Values.Sum(c => c.ResponseTime);
        //    return healthCheckResponse;

        //}

        public async Task<HealthCheckResponse> GetHealthAsync(CancellationToken cancellationToken = default)
        {
            var healthCheckResponse = new HealthCheckResponse();
            var tasks = _healthChecksBuilder.HealthChecks.Select(c => new { name = c.Key, result = c.Value.CheckHealthAsync(cancellationToken) });
            var sw = new Stopwatch();

            foreach (var task in tasks)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    sw.Reset();
                    sw.Start();
                    var result = await task.result;
                    sw.Stop();

                    healthCheckResponse.Entries.Add(task.name, new HealthCheckResultExtended(result) { ResponseTime = sw.ElapsedMilliseconds });
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    healthCheckResponse.Entries.Add(task.name, new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)));
                }
            }

            return healthCheckResponse;
        }

        public async Task<HealthCheckResultExtended> GetHealthAsync(string healthCheckName)
        {
            if (!_healthChecksBuilder.HealthChecks.TryGetValue(healthCheckName, out var healthCheck))
            {
                return null;
            }

            try
            {
                var sw = new Stopwatch();
                sw.Reset();
                sw.Start();
                var result = await healthCheck.CheckHealthAsync();

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