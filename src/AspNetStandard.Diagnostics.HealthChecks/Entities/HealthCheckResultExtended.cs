using Newtonsoft.Json;
using System;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    internal class HealthCheckResultExtended : HealthCheckResult
    {
        public HealthCheckResultExtended(HealthCheckResult healthCheckResult) :
            base(healthCheckResult.Status, healthCheckResult.Description, healthCheckResult.Exception)
        {
            LastExecutionUtc = DateTime.UtcNow;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? ResponseTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime LastExecutionUtc { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var item = obj as HealthCheckResultExtended;
            return (
                this.Description == item.Description
                && this.Exception == item.Exception
                && this.Status == item.Status
            );
        }
    }
}