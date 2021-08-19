using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecksWcf;

namespace AspNetStandard.Diagnostics.HealthChecks.Wfc
{
    public sealed class WcfHealthCheckBuilder
    {
        public WcfHealthCheckBuilder AddCheck(string name, IHealthCheck healthCheck)
        {
            WcfHealthCheckConfiguration.HealthChecksDependencies.Add(name, healthCheck);

            return this;
        }

        public WcfHealthCheckBuilder ClearHealthChecks()
        {
            WcfHealthCheckConfiguration.HealthChecksDependencies.Clear();
            
            return this;
        }
    }
}
