using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Xunit;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Services.Tests
{
    public class HealthCheckServiceTests
    {
        private readonly Mock<IHealthCheck> _healthyHealthCheckMock = new Mock<IHealthCheck>();
        private readonly Mock<IHealthCheck> _degradedHealthCheckMock = new Mock<IHealthCheck>();
        private readonly Mock<IHealthCheck> _unhealthyHealthCheckMock = new Mock<IHealthCheck>();
        private readonly Mock<IHealthCheck> _throwableHealthCheckMock = new Mock<IHealthCheck>();
        private readonly Mock<IDependencyResolver> _dependencyResolverMock = new Mock<IDependencyResolver>();
        private readonly HealthChecksBuilder _healthCheckBuilder = new HealthChecksBuilder();

        public HealthCheckServiceTests()
        {
            _healthyHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "AnyDescription")));
            _degradedHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Degraded, "AnyDescription")));
            _unhealthyHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "AnyDescription")));
            _throwableHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).ThrowsAsync(new Exception("AnyException"));
        }

        [Fact(DisplayName = "Should return correct status code if builder remain not modified")]
        public void ShouldGetStandardStatusCode()
        {
            var sut = _healthCheckBuilder.HealthCheckConfig;

            var actDegradedStatusCode = sut.GetStatusCode(HealthStatus.Degraded);
            var actHealthyStatusCode = sut.GetStatusCode(HealthStatus.Healthy);
            var actUnhealthyStatusCode = sut.GetStatusCode(HealthStatus.Unhealthy);

            Assert.True(actHealthyStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(actDegradedStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(actUnhealthyStatusCode == System.Net.HttpStatusCode.ServiceUnavailable);
        }

        [Fact(DisplayName = "Should return correct custom status code if different status code was injected")]
        public void ShouldGetCustomStatusCode()
        {
            var sut = new HealthChecksBuilder()
                            .OverrideResultStatusCodes(
                                System.Net.HttpStatusCode.NotFound,
                                System.Net.HttpStatusCode.InternalServerError,
                                System.Net.HttpStatusCode.Created)
                            .HealthCheckConfig;

            var actDegradedStatusCode = sut.GetStatusCode(HealthStatus.Degraded);
            var actHealthyStatusCode = sut.GetStatusCode(HealthStatus.Healthy);
            var actUnhealthyStatusCode = sut.GetStatusCode(HealthStatus.Unhealthy);

            Assert.True(actHealthyStatusCode == System.Net.HttpStatusCode.NotFound);
            Assert.True(actDegradedStatusCode == System.Net.HttpStatusCode.InternalServerError);
            Assert.True(actUnhealthyStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact(DisplayName = "Should return correct healthCheckResponse properties")]
        public async Task ShouldGetHealthCheckAsync()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _healthyHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            var actResponse = await sut.GetHealthAsync();

            var expectedResponse = new HealthCheckResponse();
            expectedResponse.HealthChecks.Add("AnyImplementation", new HealthCheckResultExtended(await _healthyHealthCheckMock.Object.CheckHealthAsync()));

            Assert.Equal(expectedResponse.OverAllStatus, actResponse.OverAllStatus);
            Assert.Equal(expectedResponse.HealthChecks, actResponse.HealthChecks);
        }

        [Fact(DisplayName = "Should cancel check if CancellationToken if token was canceled")]
        public void ShouldCancelCheck()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _healthyHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Func<Task> act = () => sut.GetHealthAsync(token);

            tokenSource.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(act);
        }

        [Fact(DisplayName = "Should return unhealthy response if GetHealth throws exception")]
        public async Task ShouldReturnUnhealthyIfThrows()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _throwableHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            var actResponse = await sut.GetHealthAsync();

            var expectedResponse = new HealthCheckResponse();
            expectedResponse.HealthChecks.Add("AnyImplementation", new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)));

            Assert.Equal(HealthStatus.Unhealthy, actResponse.OverAllStatus);
            Assert.Equal(expectedResponse.HealthChecks, actResponse.HealthChecks);
        }

        [Fact(DisplayName = "Should return degraded status if at least one check is degraded")]
        public async Task ShouldReturnDegradedOverAll()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _healthyHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            var actResponse = await sut.GetHealthAsync();

            var expectedResponse = new HealthCheckResponse();
            expectedResponse.HealthChecks.Add("AnyImplementation", new HealthCheckResultExtended(await _degradedHealthCheckMock.Object.CheckHealthAsync()));
            expectedResponse.HealthChecks.Add("AnotherImplementation", new HealthCheckResultExtended(await _healthyHealthCheckMock.Object.CheckHealthAsync()));

            Assert.Equal(HealthStatus.Degraded, actResponse.OverAllStatus);
            Assert.Equal(expectedResponse.HealthChecks, actResponse.HealthChecks);
        }

        [Fact(DisplayName = "Should return unhealthy status if at least one check is unhealthy")]
        public async Task ShouldReturnUnhealthyOverAll()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _unhealthyHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            var actResponse = await sut.GetHealthAsync();

            var expectedResponse = new HealthCheckResponse();
            expectedResponse.HealthChecks.Add("AnyImplementation", new HealthCheckResultExtended(await _degradedHealthCheckMock.Object.CheckHealthAsync()));
            expectedResponse.HealthChecks.Add("AnotherImplementation", new HealthCheckResultExtended(await _unhealthyHealthCheckMock.Object.CheckHealthAsync()));

            Assert.Equal(HealthStatus.Unhealthy, actResponse.OverAllStatus);
            Assert.Equal(expectedResponse.HealthChecks, actResponse.HealthChecks);
        }

        [Fact(DisplayName = "Should check specific health check searched by name")]
        public async Task ShouldCheckByName()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _unhealthyHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            var actResponse = await sut.GetHealthAsync("AnotherImplementation");

            var expectedResponse = new HealthCheckResultExtended(await _unhealthyHealthCheckMock.Object.CheckHealthAsync());

            Assert.Equal(expectedResponse, actResponse);
        }

        [Fact(DisplayName = "Should return unhealthy if specific HC throws")]
        public async Task ShouldReturnUnhealthyIfSearchedCheckThrows()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _throwableHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            var actResponse = await sut.GetHealthAsync("AnotherImplementation");

            var expectedResponse = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy));

            Assert.Equal(expectedResponse, actResponse);
        }

        [Fact(DisplayName = "Should throw error if specific HC not founded")]
        public void ShouldReturnNullIfSearchedCheckNotFound()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _throwableHealthCheckMock.Object)
                .HealthCheckConfig
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_dependencyResolverMock.Object, hcDependencies);

            Func<Task> act = () => sut.GetHealthAsync("ImplementationThatDoesntExists");

            Assert.ThrowsAsync<OperationCanceledException>(act);
        }
    }
}