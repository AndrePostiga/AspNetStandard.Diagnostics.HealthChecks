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
        }

        public HealthStatus Status { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Exception Exception { get; }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var item = obj as HealthCheckResult;
            return (
                this.Description == item.Description
                && this.Exception == item.Exception
                && this.Status == item.Status
            );
        }
    }
}