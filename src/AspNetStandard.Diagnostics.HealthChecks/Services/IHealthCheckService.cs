using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal interface IHealthCheckService
    {
        Task<HealthCheckResponse> GetHealthAsync(CancellationToken cancellationToken = default);

        Task<HealthCheckResultExtended> GetHealthAsync(string healthCheckName, CancellationToken cancellationToken = default);
    }
}