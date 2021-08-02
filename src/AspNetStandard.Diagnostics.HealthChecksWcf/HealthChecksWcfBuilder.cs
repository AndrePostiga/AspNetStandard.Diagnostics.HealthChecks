using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public class HealthChecksWcfBuilder
    {
        public HealthChecksWcfBuilder AddCheck(string name, IHealthCheck healthCheck)
        {
            HealthCheckWcfConfiguration.HealthChecksDependencies
                .Add(name, healthCheck);

            return this;
        }

        public HealthChecksWcfBuilder AddWcf()
        {
            HealthCheckWcfConfiguration.HealthChecksDependencies
                .Add("API", new HealthCheckWcf());
            return this;
        }
    }
}
