using System;
using System.Threading.Tasks;
using AspNetStandard.Diagnostics.HealthChecks;

namespace WebApi.HealthChecks.Test.Implementations
{
    public class ExceptionHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync()
        {
            return await Task.FromException<HealthCheckResult>(new InvalidOperationException("The service is down."));
        }
    }
}