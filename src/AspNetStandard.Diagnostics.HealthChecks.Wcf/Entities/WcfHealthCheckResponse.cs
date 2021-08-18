using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AspNetStandard.Diagnostics.HealthChecks.Wfc.Entities
{
    [DataContract]
    public sealed class WcfHealthCheckResponse
    {
        [DataMember]
        public string OverAllStatus { get; set; }

        [DataMember]
        public IDictionary<string, WcfHealthCheckResult> HealthChecks { get; set; }

        public WcfHealthCheckResponse()
        {
            HealthChecks = new Dictionary<string, WcfHealthCheckResult>();
        }

        public string GetOverallStatus()
        {
            if (HealthChecks.Values.Any(x => x.Status == nameof(HealthStatus.Unhealthy)))
                return nameof(HealthStatus.Unhealthy);

            if (HealthChecks.Values.Any(x => x.Status == nameof(HealthStatus.Degraded)))
                return nameof(HealthStatus.Degraded);

            return nameof(HealthStatus.Healthy);
        }

        public void GenerateWcfResult(string key, HealthCheckResult result)
        {
            HealthChecks.Add(key, new WcfHealthCheckResult
            {
                Status = result.Status.ToString(),
                Description = result.Description,
                Exception = new WcfExceptionHandler(result.Exception).Handler()
            });
        }
    }
}
