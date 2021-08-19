using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    internal static class WcfHealthCheckConfiguration
    {
        public static IDictionary<string, IHealthCheck> HealthChecksDependencies { get; }
            = new Dictionary<string, IHealthCheck>(StringComparer.OrdinalIgnoreCase);
    }
}
