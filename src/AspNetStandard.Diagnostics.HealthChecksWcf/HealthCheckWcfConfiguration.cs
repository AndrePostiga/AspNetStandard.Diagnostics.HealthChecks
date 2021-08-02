using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public static class HealthCheckWcfConfiguration
    {
        public static IDictionary<string, IHealthCheck> HealthChecksDependencies { get; }
            = new Dictionary<string, IHealthCheck>(
                StringComparer.OrdinalIgnoreCase
              );

    }
}
