using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: InternalsVisibleTo("AspNetStandard.Diagnostics.HealthChecks.Tests")]

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal class HealthCheckService : IHealthCheckService
    {
        private HealthChecksBuilder _healthChecksBuilder { get; }

        private readonly HttpConfiguration _httpConfiguration;
        private IDictionary<string, IHealthCheck> _healthChecksInstances { get => ResolveDependenciesOfHealthChecks(); }

        public HealthCheckService(HttpConfiguration httpConfiguration, HealthChecksBuilder healthChecksBuilder)
        {
            _healthChecksBuilder = healthChecksBuilder;
            _httpConfiguration = httpConfiguration;
        }

        // isso nem era pra estar nos services, refatorar pro handler
        public HttpStatusCode GetStatusCode(HealthStatus healthstatus)
        {
            return _healthChecksBuilder.ResultStatusCodes[healthstatus];
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
            var tasks = _healthChecksInstances.Select(c => new { name = c.Key, result = c.Value.CheckHealthAsync(cancellationToken) });
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
            if (!_healthChecksInstances.TryGetValue(healthCheckName, out var healthCheck))
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

        private IDictionary<string, IHealthCheck> ResolveDependenciesOfHealthChecks()
        {
            using (var dependencyScope = _httpConfiguration.DependencyResolver.BeginScope())
            {
                var result = new Dictionary<string, IHealthCheck>();

                foreach (var registration in _healthChecksBuilder.HealthChecks)
                {
                    if (registration.Value.IsSingleton)
                    {
                        result.Add(registration.Key, registration.Value.Instance);
                    }
                    else
                    {
                        var instance = (IHealthCheck)dependencyScope.GetService(registration.Value.Type);

                        result.Add(registration.Key, instance);
                    }
                }

                return result;
            }
        }
    }
}