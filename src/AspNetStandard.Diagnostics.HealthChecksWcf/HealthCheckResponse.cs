using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    [DataContract]
    public class HealthCheckResponse
    {
        [DataMember]
        public string Status { get; set; }

        public HealthCheckResponse()
        {
            HealthChecks = new Dictionary<string, HealthCheckResultExtended>();
        }

        [DataMember]
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
