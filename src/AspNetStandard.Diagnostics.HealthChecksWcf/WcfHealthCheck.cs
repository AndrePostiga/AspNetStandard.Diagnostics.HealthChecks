using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public class WcfHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => {
               return new HealthCheckResult(HealthStatus.Healthy, "The Api is Healthy");
            });
        }
    }
}
