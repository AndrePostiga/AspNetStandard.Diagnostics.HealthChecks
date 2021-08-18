using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.Wfc
{
    public sealed class WcfHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => new HealthCheckResult(HealthStatus.Healthy, "The Api is Healthy"));
        }
    }
}
