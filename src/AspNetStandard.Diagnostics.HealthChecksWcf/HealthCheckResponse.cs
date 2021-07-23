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
    }
}
