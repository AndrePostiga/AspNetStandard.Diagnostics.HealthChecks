using System.Threading.Tasks;
using WebApi.HealthChecks.Test.Services;
using AspNetStandard.Diagnostics.HealthChecks;
using System.Threading;
using AspNetStandard.Diagnostics.HealthChecks.Entities;

namespace WebApi.HealthChecks.Test.Implementations
{
    public class DegradedHealthCheck : IHealthCheck
    {
        private readonly ICosmosClient _cosmosClient;

        public DegradedHealthCheck(ICosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            return new HealthCheckResult(await _cosmosClient.Connect() ? HealthStatus.Healthy : HealthStatus.Degraded);
        }
    }
}