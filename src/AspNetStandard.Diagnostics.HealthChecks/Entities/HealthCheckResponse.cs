using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetStandard.Diagnostics.HealthChecks.Entities
{
    class HealthCheckResponse
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
