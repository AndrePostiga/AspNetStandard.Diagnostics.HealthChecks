using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public sealed class HealthChecksBuilder
    {
        private HealthCheckConfiguration _hcConfig = new HealthCheckConfiguration();

        public HealthCheckConfiguration Build()
        {
            HealthCheckConfiguration hcConfig = _hcConfig;
            Reset();
            return hcConfig;
        }

        public void Reset()
        {
            _hcConfig = new HealthCheckConfiguration();
        }

        public HealthChecksBuilder AddCheck(string name, IHealthCheck healthCheck)
        {
            _hcConfig.HealthChecksDependencies.Add(name, new Registration(healthCheck));
            return this;
        }
        public HealthChecksBuilder AddCheck<T>(string name) where T : IHealthCheck
        {
            _hcConfig.HealthChecksDependencies.Add(name, new Registration(typeof(T)));
            return this;
        }
        public HealthChecksBuilder AddCheck(string name, Func<HealthCheckResult> check)
        {
            _hcConfig.HealthChecksDependencies.Add(name, new Registration(new LambdaHealthCheck(check)));
            return this;
        }
        public HealthChecksBuilder OverrideResultStatusCodes(HttpStatusCode healthy = HttpStatusCode.OK, HttpStatusCode degraded = HttpStatusCode.OK, HttpStatusCode unhealthy = HttpStatusCode.ServiceUnavailable)
        {
            _hcConfig.ResultStatusCodes[HealthStatus.Healthy] = healthy;
            _hcConfig.ResultStatusCodes[HealthStatus.Degraded] = degraded;
            _hcConfig.ResultStatusCodes[HealthStatus.Unhealthy] = unhealthy;
            return this;
        }
        public HealthChecksBuilder UseAuthorization(string apiKey)
        {
            _hcConfig.ApiKey = apiKey;
            return this;
        }
        public HealthChecksBuilder ConfigureJsonSerializerSettings(JsonSerializerSettings settings)
        {
            _hcConfig.SerializerSettings = settings;
            return this;
        }
    }
}