using System.Collections.Generic;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    internal class HealthCheckResponse
    {
        public HealthCheckResponse()
        {
            Entries = new Dictionary<string, HealthCheckResultExtended>();
        }

        public HealthStatus OverAllStatus { get; set; }
        public long? TotalResponseTime { get; set; }
        public IDictionary<string, HealthCheckResultExtended> Entries { get; }
    }
}