using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    internal interface IHealthCheckConfiguration
    {
        IDictionary<HealthStatus, HttpStatusCode> ResultStatusCodes { get; }
        IDictionary<string, Registration> HealthChecksDependencies { get; }
        string ApiKey { get; set; }
        JsonSerializerSettings SerializerSettings { get; set; }
        ILogger Logger { get; set; }
        HttpStatusCode GetStatusCode(HealthStatus healthstatus);
    }
}