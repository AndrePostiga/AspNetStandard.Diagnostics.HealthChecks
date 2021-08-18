using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AspNetStandard.Diagnostics.HealthChecks.Wfc.Entities
{
    [DataContract]
    public sealed class WcfHealthCheckResult
    {
        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ExceptionWcf Exception { get; set; }
    }

    public sealed class ExceptionWcf
    {
        public List<Error> Errors { get; set; }

        public string Message { get; set; }

        public object InnerException { get; set; }

        public string StackTraceString { get; set; }

        public string Type { get; set; }

        public string HelpLink { get; set; }
    }

    public sealed class Error
    {
        public string Source { get; set; }
    }
}
