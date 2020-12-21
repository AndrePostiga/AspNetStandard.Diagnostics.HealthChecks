using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetStandard.Diagnostics.HealthChecks;
using AspNetStandard.Diagnostics.HealthChecks.Entities;

namespace WebApi.HealthChecks.Test.Implementations
{
    public class ExceptionHealthCheck : IHealthCheck
    {        
        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromException<HealthCheckResult>(new InvalidOperationException("The service is down."));
        }
    }
}