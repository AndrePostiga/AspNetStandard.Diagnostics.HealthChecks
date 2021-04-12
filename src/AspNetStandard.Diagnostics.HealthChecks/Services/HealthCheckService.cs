using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Errors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

[assembly: InternalsVisibleTo("AspNetStandard.Diagnostics.HealthChecks.Tests")]

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal class HealthCheckService : IHealthCheckService
    {
        private IDependencyResolver _dependencyResolver;
        private IDictionary<string, Registration> _registeredChecks;

        public HealthCheckService(IDependencyResolver dependencyResolver, IDictionary<string, Registration> registeredChecks)
        {
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            _registeredChecks = registeredChecks ?? throw new ArgumentNullException(nameof(registeredChecks));
        }

        public async Task<HealthCheckResponse> GetHealthAsync(CancellationToken cancellationToken = default)
        {
            var healthCheckResponse = new HealthCheckResponse();
            var healthChecks = ResolveDependencies();
            var sw = new Stopwatch();            

            foreach (var task in healthChecks)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    sw.Reset();
                    sw.Start();
                    var result = await task.Value.CheckHealthAsync(cancellationToken);
                    sw.Stop();

                    healthCheckResponse.HealthChecks.Add(task.Key, new HealthCheckResultExtended(result) { ResponseTime = sw.ElapsedMilliseconds });
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    healthCheckResponse.HealthChecks.Add(task.Key, new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)));
                }
            }

            return healthCheckResponse;
        }

        public async Task<HealthCheckResultExtended> GetHealthAsync(string healthCheckName, CancellationToken cancellationToken = default)
        {
            if (!_registeredChecks.TryGetValue(healthCheckName, out Registration healthCheckRegistration))
            {
                throw new NotFoundError(healthCheckName);
            }

            var healthCheck = ResolveDependencies(healthCheckRegistration);

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

        private Dictionary<string, IHealthCheck> ResolveDependencies()
        {
            var result = new Dictionary<string, IHealthCheck>();

            _dependencyResolver.BeginScope();
            foreach (var registration in _registeredChecks)
            {
                result.Add(
                    registration.Key,
                    ResolveDependencies(registration.Value)
                );
            }

            return result;
        }

        private IHealthCheck ResolveDependencies(Registration dependency)
        {
            if (dependency.IsSingleton)
            {
                return dependency.Instance;
            }

            return (IHealthCheck)_dependencyResolver.GetService(dependency.Type);            
        }


    }
}