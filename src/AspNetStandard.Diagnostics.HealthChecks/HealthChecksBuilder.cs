using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public class HealthChecksBuilder
    {
        internal HealthChecksBuilder()
        {
        }

        internal IDictionary<HealthStatus, HttpStatusCode> ResultStatusCodes { get; } = new Dictionary<HealthStatus, HttpStatusCode>(3)
        {
            {HealthStatus.Healthy, HttpStatusCode.OK},
            {HealthStatus.Degraded, HttpStatusCode.OK},
            {HealthStatus.Unhealthy, HttpStatusCode.ServiceUnavailable}
        };

        internal IDictionary<string, Registration> HealthChecksDependencies { get; } = new Dictionary<string, Registration>(StringComparer.OrdinalIgnoreCase);

        internal IDictionary<string, IHealthCheck> HealthChecks { get; set; }

        internal bool AddWarningHeader { get; private set; } = true;

        public HealthChecksBuilder AddCheck(string name, IHealthCheck healthCheck)
        {
            HealthChecksDependencies.Add(name, new Registration(healthCheck));

            return this;
        }

        public HealthChecksBuilder AddCheck<T>(string name) where T : IHealthCheck
        {
            HealthChecksDependencies.Add(name, new Registration(typeof(T)));

            return this;
        }

        public HealthChecksBuilder AddCheck(string name, Func<HealthCheckResult> check)
        {
            HealthChecksDependencies.Add(name, new Registration(new LambdaHealthCheck(check)));

            return this;
        }

        public HealthChecksBuilder OverrideResultStatusCodes(HttpStatusCode healthy = HttpStatusCode.OK, HttpStatusCode degraded = HttpStatusCode.OK, HttpStatusCode unhealthy = HttpStatusCode.ServiceUnavailable)
        {
            ResultStatusCodes[HealthStatus.Healthy] = healthy;
            ResultStatusCodes[HealthStatus.Degraded] = degraded;
            ResultStatusCodes[HealthStatus.Unhealthy] = unhealthy;

            return this;
        }

        internal string ApiKey = null; // ToDo: Sendo um atributo, tá fora de padrão. Transformar em propriedade?

        public HealthChecksBuilder UseAuthorization(string apiKey)
        {
            ApiKey = apiKey;
            return this;
        }
    }
}