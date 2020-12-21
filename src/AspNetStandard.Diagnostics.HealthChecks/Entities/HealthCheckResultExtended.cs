using Newtonsoft.Json;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    internal class HealthCheckResultExtended : HealthCheckResult
    {
        public HealthCheckResultExtended(HealthCheckResult healthCheckResult) :
            base(healthCheckResult.Status, healthCheckResult.Description, healthCheckResult.Exception)
        {

        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? ResponseTime { get; set; }
    }
}