using System.Collections.Generic;
using System.Linq;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    public class HealthCheckResponse
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
                var status = HealthStatus.Healthy;
                foreach (var healthCheckResultExtended in Entries.Values)
                {
                    if (healthCheckResultExtended.Status == HealthStatus.Unhealthy)
                    {
                        status = HealthStatus.Unhealthy;
                        break;
                    }

                    if (healthCheckResultExtended.Status == HealthStatus.Degraded)
                    {
                        status = HealthStatus.Degraded;
                    }
                }
                return status;
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