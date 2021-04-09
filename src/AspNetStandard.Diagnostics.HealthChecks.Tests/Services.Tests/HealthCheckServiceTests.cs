using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using AspNetStandard.Diagnostics.HealthChecks;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http.Dependencies;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Services.Tests
{  
    public class HealthCheckServiceTests
    {
        Mock<IHealthCheck> _healthyHealthCheckMock = new Mock<IHealthCheck>();
        Mock<IHealthCheck> _degradedHealthCheckMock = new Mock<IHealthCheck>();
        Mock<IHealthCheck> _unhealthyHealthCheckMock = new Mock<IHealthCheck>();
        Mock<IHealthCheck> _ThrowableHealthCheckMock = new Mock<IHealthCheck>();
        Mock<IDependencyResolver> _DependencyResolverMock = new Mock<IDependencyResolver>();
        HealthChecksBuilder _healthCheckBuilder = new HealthChecksBuilder();

        public HealthCheckServiceTests()
        {
            _healthyHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "AnyDescription", null)));
            _degradedHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Degraded, "AnyDescription", null)));
            _unhealthyHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "AnyDescription", null)));
            _ThrowableHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).ThrowsAsync(new Exception("AnyException"));
            //_DependencyResolverMock.Setup(x => x.GetService(It.IsAny<Type>())).Returns()
        }
         
        [Fact(DisplayName = "Should return correct status code if builder remain not modified")]
        public void ShouldGetStandardStatusCode()
        {

            var sut = _healthCheckBuilder.Build();

            var ActDegradedStatusCode = sut.GetStatusCode(HealthStatus.Degraded);
            var ActHealthyStatusCode = sut.GetStatusCode(HealthStatus.Healthy);
            var ActUnhealthyStatusCode = sut.GetStatusCode(HealthStatus.Unhealthy);

            Assert.True(ActHealthyStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(ActDegradedStatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(ActUnhealthyStatusCode == System.Net.HttpStatusCode.ServiceUnavailable);
        }

        [Fact(DisplayName = "Should return correct custom status code if diferent status code was injected")]
        public void ShouldGetCustomStatusCode()
        {
            var sut = new HealthChecksBuilder()
                            .OverrideResultStatusCodes(
                                System.Net.HttpStatusCode.NotFound,
                                System.Net.HttpStatusCode.InternalServerError,
                                System.Net.HttpStatusCode.Created)
                            .Build();

            var ActDegradedStatusCode = sut.GetStatusCode(HealthStatus.Degraded);
            var ActHealthyStatusCode = sut.GetStatusCode(HealthStatus.Healthy);
            var ActUnhealthyStatusCode = sut.GetStatusCode(HealthStatus.Unhealthy);

            Assert.True(ActHealthyStatusCode == System.Net.HttpStatusCode.NotFound);
            Assert.True(ActDegradedStatusCode == System.Net.HttpStatusCode.InternalServerError);
            Assert.True(ActUnhealthyStatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact(DisplayName = "Should return correct healthCheckResponse properties")]
        public async Task ShouldGetHealthCheckAsync()
        {
            var hcDependencies = _healthCheckBuilder.AddCheck("AnyImplementation", _healthyHealthCheckMock.Object).Build().HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(await _healthyHealthCheckMock.Object.CheckHealthAsync()));
            
            Assert.Equal(ExpectedResponse.OverAllStatus, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);            
        }

        
        [Fact(DisplayName = "Should cancel check if CancellationToken if token was canceled")]
        public void ShouldCancelCheck()
        {

            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _healthyHealthCheckMock.Object)
                .Build()
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Func<Task> act = () => sut.GetHealthAsync(token);

            tokenSource.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(act);
        }


        [Fact(DisplayName = "Should return unhealthy response if GetHealth throws exception")]
        public async Task ShouldReturnUnhealthIfThrows()
        {

            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _ThrowableHealthCheckMock.Object)
                .Build()
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)));

            Assert.Equal(HealthStatus.Unhealthy, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);
        }


        [Fact(DisplayName = "Should return degraded status if at least one check is degraded")]
        public async Task ShouldReturnDegradedOverAll()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _healthyHealthCheckMock.Object)
                .Build()
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(await _degradedHealthCheckMock.Object.CheckHealthAsync()));
            ExpectedResponse.Entries.Add("AnotherImplementation", new HealthCheckResultExtended(await _healthyHealthCheckMock.Object.CheckHealthAsync()));

            Assert.Equal(HealthStatus.Degraded, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);
        }

        [Fact(DisplayName = "Should return unhealthy status if at least one check is unhealthy")]
        public async Task ShouldReturnUnhealthyOverAll()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _unhealthyHealthCheckMock.Object)
                .Build()
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(await _degradedHealthCheckMock.Object.CheckHealthAsync()));
            ExpectedResponse.Entries.Add("AnotherImplementation", new HealthCheckResultExtended(await _unhealthyHealthCheckMock.Object.CheckHealthAsync()));

            Assert.Equal(HealthStatus.Unhealthy, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);
        }


        [Fact(DisplayName = "Should check specific healthcheck searched by name")]
        public async Task ShouldCheckByName()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _unhealthyHealthCheckMock.Object)
                .Build()
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            var ActResponse = await sut.GetHealthAsync("AnotherImplementation");

            var ExpectedResponse = new HealthCheckResultExtended(await _unhealthyHealthCheckMock.Object.CheckHealthAsync());            
            
            Assert.Equal(ExpectedResponse, ActResponse);
        }

        [Fact(DisplayName = "Should return unhealthy if specific HC throws")]
        public async Task ShouldReturnUnhealthyIfSearchedCheckThrows()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _ThrowableHealthCheckMock.Object)
                .Build()
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            var ActResponse = await sut.GetHealthAsync("AnotherImplementation");

            var ExpectedResponse = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy));

            Assert.Equal(ExpectedResponse, ActResponse);
        }


        [Fact(DisplayName = "Should throw error if specific HC not founded")]
        public void ShouldReturnNullIfSearchedCheckNotFound()
        {
            var hcDependencies = _healthCheckBuilder
                .AddCheck("AnyImplementation", _degradedHealthCheckMock.Object)
                .AddCheck("AnotherImplementation", _ThrowableHealthCheckMock.Object)
                .Build()
                .HealthChecksDependencies;

            var sut = new HealthCheckService(_DependencyResolverMock.Object, hcDependencies);

            Func<Task> act = () => sut.GetHealthAsync("ImplementationThatDoesntExists");

            Assert.ThrowsAsync<OperationCanceledException>(act);
        }

    }
}
