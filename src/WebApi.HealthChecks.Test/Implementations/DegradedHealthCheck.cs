﻿using System.Threading.Tasks;

namespace WebApi.HealthChecks.Test.Implementations
{
    public class DegradedHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync()
        {
            return await Task.FromResult(new HealthCheckResult(HealthStatus.Degraded));
        }
    }
}