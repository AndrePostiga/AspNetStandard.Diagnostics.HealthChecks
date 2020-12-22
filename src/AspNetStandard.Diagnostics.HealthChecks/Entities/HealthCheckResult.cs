using Newtonsoft.Json;
using System;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    public class HealthCheckResult
    {
        public HealthCheckResult(HealthStatus status, string description = null, Exception exception = null)
        {
            Status = status;
            Description = description;
            Exception = exception;
            LastExecutionUtc = DateTime.UtcNow;
        }

        public HealthStatus Status { get; }

        public DateTime LastExecutionUtc { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Exception Exception { get; }
    }
}