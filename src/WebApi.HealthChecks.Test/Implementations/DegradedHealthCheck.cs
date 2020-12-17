using System.Threading.Tasks;
using WebApi.HealthChecks.Test.Services;
using AspNetStandard.Diagnostics.HealthChecks;

namespace WebApi.HealthChecks.Test.Implementations
{
    public class DegradedHealthCheck : IHealthCheck
    {
        private readonly ICosmosClient _cosmosClient;

        public DegradedHealthCheck(ICosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync()
        {
            return new HealthCheckResult(await _cosmosClient.Connect() ? HealthStatus.Healthy : HealthStatus.Degraded);
        }
    }
}