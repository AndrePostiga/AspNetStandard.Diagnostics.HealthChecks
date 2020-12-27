using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    public interface IHealthCheck
    {
        Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    }
}