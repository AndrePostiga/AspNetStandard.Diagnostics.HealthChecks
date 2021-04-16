using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    internal class HealthCheckConfiguration : IHealthCheckConfiguration
    {
        public IDictionary<HealthStatus, HttpStatusCode> ResultStatusCodes { get; } = new Dictionary<HealthStatus, HttpStatusCode>(3)
        {
            {HealthStatus.Healthy, HttpStatusCode.OK},
            {HealthStatus.Degraded, HttpStatusCode.OK},
            {HealthStatus.Unhealthy, HttpStatusCode.ServiceUnavailable}
        };
        private ILogger _logger;

        public ILogger Logger
        {
            get => _logger;
            set
            {
                if (value == null) return;
                _logger = value;
            }
        }

        public IDictionary<string, Registration> HealthChecksDependencies { get; } = new Dictionary<string, Registration>(StringComparer.OrdinalIgnoreCase);

        public string ApiKey { get; set; }


        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
                {
                    ProcessDictionaryKeys = true
                }
            }            
        };

        public JsonSerializerSettings SerializerSettings
        {
            get => _serializerSettings;
            set
            {
                if (value == null) return;                
                _serializerSettings = value;
            }
        }

        public HttpStatusCode GetStatusCode(HealthStatus healthstatus)
        {
            return ResultStatusCodes[healthstatus];
        }

    }
}

