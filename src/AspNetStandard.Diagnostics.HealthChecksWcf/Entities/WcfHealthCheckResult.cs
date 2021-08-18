using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AspNetStandard.Diagnostics.HealthChecksWcf.Entities
{

    [DataContract]
    public class WcfHealthCheckResult
    {
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public ExceptionWcf Exception { get; set; }

    }

    public class ExceptionWcf
    {
        public List<Error> Errors { get; set; }
        public string Message { get; set; }
        public object InnerException { get; set; }
        public string StackTraceString { get; set; }
        public string Type { get; set; }
        public string HelpLink { get; set; }

    }

    public class Error
    {
        public string source { get; set; }
    }

}
