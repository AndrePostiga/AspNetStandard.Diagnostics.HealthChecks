using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Tests.Utils.Builders;
using AspNetStandard.Diagnostics.HealthChecks.Wfc;
using AspNetStandard.Diagnostics.HealthChecksWcf;
using Moq;
using System;
using System.Threading;
using Xunit;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.HealthChecks.Tests
{
    public class HealthCheckWcfTests
    {
        private readonly WcfHealthCheckService _wcfHealthCheckService = new WcfHealthCheckService();

        [Fact(DisplayName = "Should return healthy given successful execution")]
        public void ShouldReturnHealthyOnSuccessfulExecution()
        {
            new WcfHealthCheckBuilder().ClearHealthChecks()
                                       .AddCheck("wcf", new WcfHealthCheck());

            var response = _wcfHealthCheckService.ExecuteHealthCheck();

            var wcfHealthcheck = response.HealthChecks["wcf"];
            Assert.Equal("Healthy", response.OverAllStatus);
            Assert.Equal(1, response.HealthChecks.Count);

            Assert.Equal("The Api is Healthy", wcfHealthcheck.Description);
            Assert.Equal("Healthy", wcfHealthcheck.Status);
            Assert.Null(wcfHealthcheck.Exception);
        }

        [Fact(DisplayName = "Should return unhealthy given execution with error")]
        public void ShouldReturnUnhealthyGivenRunWithError()
        {
            var healthCheck = new HealthCheckBuilder().BuildDefault()
                                                      .WithHealthCheckResult(HealthStatus.Unhealthy,
                                                                             "The Api is Unhealthy",
                                                                             new NotImplementedException())
                                                      .Create();

            new WcfHealthCheckBuilder().ClearHealthChecks()
                                       .AddCheck("healthcheck", healthCheck);

            var response = _wcfHealthCheckService.ExecuteHealthCheck();

            var wcfHealthCheck = response.HealthChecks["healthcheck"];
            Assert.Equal("Unhealthy", response.OverAllStatus);
            Assert.Equal(1, response.HealthChecks.Count);

            Assert.Equal("The Api is Unhealthy", wcfHealthCheck.Description);
            Assert.Equal("Unhealthy", wcfHealthCheck.Status);
            Assert.NotNull(wcfHealthCheck.Exception);
            Assert.Equal(typeof(NotImplementedException).FullName, wcfHealthCheck.Exception.Type);
        }

        [Fact(DisplayName = "Should return degraded given one healthcheck degraded")]
        public void ShouldReturnDegradedGivenRunWithError()
        {
            var healthCheck = new HealthCheckBuilder().BuildDefault()
                                                      .WithHealthCheckResult(HealthStatus.Degraded,
                                                                             "The Api is Degraded")
                                                      .Create();

            new WcfHealthCheckBuilder().ClearHealthChecks()
                                       .AddCheck("healthcheck", healthCheck);

            var response = _wcfHealthCheckService.ExecuteHealthCheck();

            var wcfHealthCheck = response.HealthChecks["healthcheck"];
            Assert.Equal("Degraded", response.OverAllStatus);
            Assert.Equal(1, response.HealthChecks.Count);

            Assert.Equal("The Api is Degraded", wcfHealthCheck.Description);
            Assert.Equal("Degraded", wcfHealthCheck.Status);
            Assert.Null(wcfHealthCheck.Exception);
        }
    }
}
