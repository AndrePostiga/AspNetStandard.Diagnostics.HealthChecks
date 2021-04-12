using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Domain.Tests
{
    public class HealthCheckResponseTests
    {
        HealthCheckResultExtended HealthyResult;
        HealthCheckResultExtended DegradedResult;
        HealthCheckResultExtended UnhealthyResult;

        public HealthCheckResponseTests()
        {
            HealthyResult = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Healthy)) { ResponseTime = 1};
            DegradedResult = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Degraded)) { ResponseTime = 1 };
            UnhealthyResult = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)) { ResponseTime = 1 };
        }

        [Fact(DisplayName = "Should return degraded if at least one status is degraded")]
        public void ShouldReturnDegraded()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", HealthyResult);
            sut.HealthChecks.Add("AnyAnotherName", DegradedResult);

            Assert.Equal(HealthStatus.Degraded, sut.OverAllStatus);

        }

        [Fact(DisplayName = "Should return unhealthy if at least one status is unhealthy")]
        public void ShouldReturnUnhealthy()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", HealthyResult);
            sut.HealthChecks.Add("AnyAnotherName", DegradedResult);
            sut.HealthChecks.Add("UnhealthyAnotherName", UnhealthyResult);

            Assert.Equal(HealthStatus.Unhealthy, sut.OverAllStatus);

        }

        [Fact(DisplayName = "Should return healthy if all status is healthy")]
        public void ShouldReturnHealthy()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", HealthyResult);
            sut.HealthChecks.Add("AnyAnotherName", HealthyResult);
            sut.HealthChecks.Add("UnhealthyAnotherName", HealthyResult);

            Assert.Equal(HealthStatus.Healthy, sut.OverAllStatus);

        }

        [Fact(DisplayName = "Should sum correctly totalResponseTime")]
        public void ShouldSumCorrectly()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", HealthyResult);
            sut.HealthChecks.Add("AnyAnotherName", DegradedResult);
            sut.HealthChecks.Add("UnhealthyAnotherName", UnhealthyResult);

            Assert.Equal(3, sut.TotalResponseTime);
        }
    }
}
