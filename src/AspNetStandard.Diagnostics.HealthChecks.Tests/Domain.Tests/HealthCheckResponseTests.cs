using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Xunit;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Domain.Tests
{
    public class HealthCheckResponseTests
    {
        private readonly HealthCheckResultExtended _healthyResult;
        private readonly HealthCheckResultExtended _degradedResult;
        private readonly HealthCheckResultExtended _unhealthyResult;

        public HealthCheckResponseTests()
        {
            _healthyResult = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Healthy)) { ResponseTime = 1 };
            _degradedResult = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Degraded)) { ResponseTime = 1 };
            _unhealthyResult = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)) { ResponseTime = 1 };
        }

        [Fact(DisplayName = "Should return degraded if at least one status is degraded")]
        public void ShouldReturnDegraded()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", _healthyResult);
            sut.HealthChecks.Add("AnyAnotherName", _degradedResult);

            Assert.Equal(HealthStatus.Degraded, sut.OverAllStatus);
        }

        [Fact(DisplayName = "Should return unhealthy if at least one status is unhealthy")]
        public void ShouldReturnUnhealthy()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", _healthyResult);
            sut.HealthChecks.Add("AnyAnotherName", _degradedResult);
            sut.HealthChecks.Add("UnhealthyAnotherName", _unhealthyResult);

            Assert.Equal(HealthStatus.Unhealthy, sut.OverAllStatus);
        }

        [Fact(DisplayName = "Should return healthy if all status is healthy")]
        public void ShouldReturnHealthy()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", _healthyResult);
            sut.HealthChecks.Add("AnyAnotherName", _healthyResult);
            sut.HealthChecks.Add("UnhealthyAnotherName", _healthyResult);

            Assert.Equal(HealthStatus.Healthy, sut.OverAllStatus);
        }

        [Fact(DisplayName = "Should sum correctly totalResponseTime")]
        public void ShouldSumCorrectly()
        {
            var sut = new HealthCheckResponse();
            sut.HealthChecks.Add("AnyName", _healthyResult);
            sut.HealthChecks.Add("AnyAnotherName", _degradedResult);
            sut.HealthChecks.Add("UnhealthyAnotherName", _unhealthyResult);

            Assert.Equal(3, sut.TotalResponseTime);
        }
    }
}