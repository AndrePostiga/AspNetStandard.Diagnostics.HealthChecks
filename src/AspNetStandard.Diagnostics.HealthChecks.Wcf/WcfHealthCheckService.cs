using AspNetStandard.Diagnostics.HealthChecks.Wfc.Entities;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public sealed class WcfHealthCheckService
    {
        public WcfHealthCheckResponse ExecuteHealthCheck()
        {
            var healthCheckResponse = new WcfHealthCheckResponse();

            foreach (var dependency in WcfHealthCheckConfiguration.HealthChecksDependencies)
            {
                var result = dependency.Value.CheckHealthAsync().Result;

                healthCheckResponse.GenerateWcfResult(dependency.Key, result);
            }

            healthCheckResponse.OverAllStatus = healthCheckResponse.GetOverallStatus();
            return healthCheckResponse;
        }
    }
}
