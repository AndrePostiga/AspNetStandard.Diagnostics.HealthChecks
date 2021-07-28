using System;
using System.Runtime.Serialization;

namespace AspNetStandard.Diagnostics.HealthChecksWcf.Entities
{
    public class WcfHealthCheckResult
    {
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Exception Exception { get; set; }
    }
}
