using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public interface IHealthCheck
    {
        Task<HealthCheckResult> CheckHealthAsync();
    }
}