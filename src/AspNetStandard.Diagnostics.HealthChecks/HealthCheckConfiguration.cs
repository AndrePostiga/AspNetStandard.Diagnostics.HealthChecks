using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public class HealthCheckConfiguration
    {
        internal IDictionary<HealthStatus, HttpStatusCode> ResultStatusCodes { get; } = new Dictionary<HealthStatus, HttpStatusCode>(3)
        {
            {HealthStatus.Healthy, HttpStatusCode.OK},
            {HealthStatus.Degraded, HttpStatusCode.OK},
            {HealthStatus.Unhealthy, HttpStatusCode.ServiceUnavailable}
        };

        internal IDictionary<string, Registration> HealthChecksDependencies { get; } = new Dictionary<string, Registration>(StringComparer.OrdinalIgnoreCase);

        private string _apiKey;
        internal string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }


        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() }
        };

        internal JsonSerializerSettings SerializerSettings
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

