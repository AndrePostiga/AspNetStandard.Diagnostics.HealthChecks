using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public class HealthCheckWcf : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => {
               return new HealthCheckResult(HealthStatus.Healthy, "ta fumegando");
            });
        }
    }
}
