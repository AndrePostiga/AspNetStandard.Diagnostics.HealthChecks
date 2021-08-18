using AspNetStandard.Diagnostics.HealthChecks.Entities;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public sealed class WcfHealthCheckBuilder
    {
        public WcfHealthCheckBuilder AddCheck(string name, IHealthCheck healthCheck)
        {
            WcfHealthCheckConfiguration.HealthChecksDependencies
                .Add(name, healthCheck);

            return this;
        }
    }
}
