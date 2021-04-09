using System.Collections.Generic;
using System.Linq;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    internal class HealthCheckResponse
    {
        public HealthCheckResponse()
        {
            Entries = new Dictionary<string, HealthCheckResultExtended>();
        }

        public IDictionary<string, HealthCheckResultExtended> Entries { get; }

        public HealthStatus OverAllStatus
        {
            get
            {
                if (Entries.Values.Any(x => x.Status == HealthStatus.Unhealthy))
                {
                    return HealthStatus.Unhealthy;
                }
                
                if (Entries.Values.Any(x => x.Status == HealthStatus.Degraded))
                {
                    return HealthStatus.Degraded;
                }
                
                return HealthStatus.Healthy;
            }
        }

        public long? TotalResponseTime
        {
            get
            {
                return Entries.Values.Sum(c => c.ResponseTime);
            }
        }
    }
}