﻿using System;
using System.Collections.Generic;
using System.Net;
using AspNetStandard.Diagnostics.HealthChecks.Entities;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public class HealthChecksBuilder
    {
        internal HealthChecksBuilder() { }

        internal IDictionary<HealthStatus, HttpStatusCode> ResultStatusCodes { get; } = new Dictionary<HealthStatus, HttpStatusCode>(3)
        {
            {HealthStatus.Healthy, HttpStatusCode.OK},
            {HealthStatus.Degraded, HttpStatusCode.OK},
            {HealthStatus.Unhealthy, HttpStatusCode.ServiceUnavailable}
        };

        internal IDictionary<string, Registration> HealthChecks { get; } = new Dictionary<string, Registration>(StringComparer.OrdinalIgnoreCase);

        internal bool AddWarningHeader { get; private set; } = true;

        public HealthChecksBuilder AddCheck(string name, IHealthCheck healthCheck)
        {
            HealthChecks.Add(name, new Registration(healthCheck));

            return this;
        }

        public HealthChecksBuilder AddCheck<T>(string name) where T : IHealthCheck
        {
            HealthChecks.Add(name, new Registration(typeof(T)));

            return this;
        }

        public HealthChecksBuilder AddCheck(string name, Func<HealthCheckResult> check)
        {
            HealthChecks.Add(name, new Registration(new LambdaHealthCheck(check)));

            return this;
        }

        public HealthChecksBuilder OverrideResultStatusCodes(HttpStatusCode healthy = HttpStatusCode.OK, HttpStatusCode degraded = HttpStatusCode.OK, HttpStatusCode unhealthy = HttpStatusCode.ServiceUnavailable)
        {
            ResultStatusCodes[HealthStatus.Healthy] = healthy;
            ResultStatusCodes[HealthStatus.Degraded] = degraded;
            ResultStatusCodes[HealthStatus.Unhealthy] = unhealthy;

            return this;
        }

        public HealthChecksBuilder Configure(bool addWarningHeader)
        {
            AddWarningHeader = addWarningHeader;

            return this;
        }
    }
}