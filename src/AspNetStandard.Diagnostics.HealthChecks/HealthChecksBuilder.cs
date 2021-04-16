using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public sealed class HealthChecksBuilder
    {
        internal HealthCheckConfiguration HealthCheckConfig { get; } = new HealthCheckConfiguration();
        public HealthChecksBuilder UseLogger(ILogger logger)
        {
            HealthCheckConfig.Logger = logger;
            return this;
        }

        public HealthChecksBuilder AddCheck(string name, IHealthCheck healthCheck)
        {
            HealthCheckConfig.HealthChecksDependencies.Add(name, new Registration(healthCheck));
            return this;
        }
        public HealthChecksBuilder AddCheck<T>(string name) where T : IHealthCheck
        {
            HealthCheckConfig.HealthChecksDependencies.Add(name, new Registration(typeof(T)));
            return this;
        }
        public HealthChecksBuilder AddCheck(string name, Func<HealthCheckResult> check)
        {
            HealthCheckConfig.HealthChecksDependencies.Add(name, new Registration(new LambdaHealthCheck(check)));
            return this;
        }
        public HealthChecksBuilder OverrideResultStatusCodes(HttpStatusCode healthy = HttpStatusCode.OK, HttpStatusCode degraded = HttpStatusCode.OK, HttpStatusCode unhealthy = HttpStatusCode.ServiceUnavailable)
        {
            HealthCheckConfig.ResultStatusCodes[HealthStatus.Healthy] = healthy;
            HealthCheckConfig.ResultStatusCodes[HealthStatus.Degraded] = degraded;
            HealthCheckConfig.ResultStatusCodes[HealthStatus.Unhealthy] = unhealthy;
            return this;
        }
        public HealthChecksBuilder UseAuthorization(string apiKey)
        {
            HealthCheckConfig.ApiKey = apiKey;
            return this;
        }
        public HealthChecksBuilder ConfigureJsonSerializerSettings(JsonSerializerSettings settings)
        {
            HealthCheckConfig.SerializerSettings = settings;
            return this;
        }
    }
}