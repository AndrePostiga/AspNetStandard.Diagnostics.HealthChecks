using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.Models
{
    internal class LambdaHealthCheck : IHealthCheck
    {
        private readonly Func<HealthCheckResult> _check;

        public LambdaHealthCheck(Func<HealthCheckResult> check)
        {
            _check = check;
        }

        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_check());
        }
    }
}