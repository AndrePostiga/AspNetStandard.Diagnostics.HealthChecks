using System.Collections.Generic;
using System.Linq;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    internal class HealthCheckResponse
    {
        public HealthCheckResponse()
        {
            HealthChecks = new Dictionary<string, HealthCheckResultExtended>();
        }
        
        public IDictionary<string, HealthCheckResultExtended> HealthChecks { get; }

        public HealthStatus OverAllStatus
        {
            get
            {
                if (HealthChecks.Values.Any(x => x.Status == HealthStatus.Unhealthy))
                {
                    return HealthStatus.Unhealthy;
                }
                
                if (HealthChecks.Values.Any(x => x.Status == HealthStatus.Degraded))
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
                return HealthChecks.Values.Sum(c => c.ResponseTime);
            }
        }
    }
}