using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Moq;
using System;
using System.Threading;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Utils.Builders
{
    internal sealed class HealthCheckBuilder
    {
        private Mock<IHealthCheck> _healthCheck;

        public HealthCheckBuilder BuildDefault()
        {
            _healthCheck = new Mock<IHealthCheck>();

            return this;
        }

        public HealthCheckBuilder WithHealthCheckResult(HealthStatus status, string description, Exception exception = default)
        {
            var healthCheckResult = new HealthCheckResult(status, description, exception);

            _healthCheck.Setup(p => p.CheckHealthAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(healthCheckResult);

            return this;
        }

        public IHealthCheck Create()
            => _healthCheck.Object;
    }
}
