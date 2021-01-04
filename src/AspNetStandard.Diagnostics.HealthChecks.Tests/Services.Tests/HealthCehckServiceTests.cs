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

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Services.Tests
{  
    public class HealthCehckServiceTests
    {
        Mock<IHealthCheck> mockHealthyHC = new Mock<IHealthCheck>();
        Mock<IHealthCheck> mockDegradedHC = new Mock<IHealthCheck>();
        Mock<IHealthCheck> mockUnhealthyHC = new Mock<IHealthCheck>();
        Mock<IHealthCheck> mockThrowsHC = new Mock<IHealthCheck>();

        public HealthCehckServiceTests()
        {
            mockHealthyHC.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "AnyDescription", null)));
            mockDegradedHC.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Degraded, "AnyDescription", null)));
            mockUnhealthyHC.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "AnyDescription", null)));
            mockThrowsHC.Setup(x => x.CheckHealthAsync(default)).ThrowsAsync(new Exception("AnyException"));
        }
         
        [Fact(DisplayName = "Should return correct status code if builder remain not modified")]
        public void ShouldGetStandardStatusCode()
        {            
            var sut = new HealthCheckService(new HealthChecksBuilder());

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
            var customBuilder = new HealthChecksBuilder();
            customBuilder.OverrideResultStatusCodes(
                System.Net.HttpStatusCode.NotFound,
                System.Net.HttpStatusCode.InternalServerError,
                System.Net.HttpStatusCode.Created
            );

            var sut = new HealthCheckService(customBuilder);

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
            var customBuilder = new HealthChecksBuilder();
            customBuilder.HealthChecks = new Dictionary<string, IHealthCheck>() {
                {"AnyImplementation", mockHealthyHC.Object }
            };            

            var sut = new HealthCheckService(customBuilder);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(await mockHealthyHC.Object.CheckHealthAsync()));
            
            Assert.Equal(ExpectedResponse.OverAllStatus, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);            
        }


        [Fact(DisplayName = "Should cancel check if CancellationToken if token was canceled")]
        public void ShouldCancelCheck()
        {
            var sut = new HealthCheckService(new HealthChecksBuilder());

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Func<Task> act = () => sut.GetHealthAsync(token);

            tokenSource.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(act);
        }


        [Fact(DisplayName = "Should return unhealthy response if GetHealth throws exception")]
        public async Task ShouldReturnUnhealthIfThrows()
        {   
            var customBuilder = new HealthChecksBuilder();
            customBuilder.HealthChecks = new Dictionary<string, IHealthCheck>() {
                {"AnyImplementation", mockThrowsHC.Object }
            };

            var sut = new HealthCheckService(customBuilder);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy)));

            Assert.Equal(HealthStatus.Unhealthy, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);
        }


        [Fact(DisplayName = "Should return degraded status if at least one check is degraded")]
        public async Task ShouldReturnDegradedOverAll()
        {
            var customBuilder = new HealthChecksBuilder();
            customBuilder.HealthChecks = new Dictionary<string, IHealthCheck>() {
                {"AnyImplementation", mockDegradedHC.Object },
                {"AnotherImplementation", mockHealthyHC.Object }
            };

            var sut = new HealthCheckService(customBuilder);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(await mockDegradedHC.Object.CheckHealthAsync()));
            ExpectedResponse.Entries.Add("AnotherImplementation", new HealthCheckResultExtended(await mockHealthyHC.Object.CheckHealthAsync()));

            Assert.Equal(HealthStatus.Degraded, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);
        }

        [Fact(DisplayName = "Should return unhealthy status if at least one check is unhealthy")]
        public async Task ShouldReturnUnhealthyOverAll()
        {
            var customBuilder = new HealthChecksBuilder();
            customBuilder.HealthChecks = new Dictionary<string, IHealthCheck>() {
                {"AnyImplementation", mockDegradedHC.Object },
                {"AnotherImplementation", mockUnhealthyHC.Object }
            };

            var sut = new HealthCheckService(customBuilder);

            var ActResponse = await sut.GetHealthAsync();

            var ExpectedResponse = new HealthCheckResponse();
            ExpectedResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(await mockDegradedHC.Object.CheckHealthAsync()));
            ExpectedResponse.Entries.Add("AnotherImplementation", new HealthCheckResultExtended(await mockUnhealthyHC.Object.CheckHealthAsync()));

            Assert.Equal(HealthStatus.Unhealthy, ActResponse.OverAllStatus);
            Assert.Equal(ExpectedResponse.Entries, ActResponse.Entries);
        }


        [Fact(DisplayName = "Should check specific healthcheck searched by name")]
        public async Task ShouldCheckByName()
        {
            var customBuilder = new HealthChecksBuilder();
            customBuilder.HealthChecks = new Dictionary<string, IHealthCheck>() {
                {"AnyImplementation", mockDegradedHC.Object },
                {"AnotherImplementation", mockUnhealthyHC.Object }
            };

            var sut = new HealthCheckService(customBuilder);

            var ActResponse = await sut.GetHealthAsync("AnotherImplementation");

            var ExpectedResponse = new HealthCheckResultExtended(await mockUnhealthyHC.Object.CheckHealthAsync());            
            
            Assert.Equal(ExpectedResponse, ActResponse);
        }

        [Fact(DisplayName = "Should return unhealthy if specific HC throws")]
        public async Task ShouldReturnUnhealthyIfSearchedCheckThrows()
        {
            var customBuilder = new HealthChecksBuilder();
            customBuilder.HealthChecks = new Dictionary<string, IHealthCheck>() {
                {"AnyImplementation", mockDegradedHC.Object },
                {"AnotherImplementation", mockThrowsHC.Object }
            };

            var sut = new HealthCheckService(customBuilder);

            var ActResponse = await sut.GetHealthAsync("AnotherImplementation");

            var ExpectedResponse = new HealthCheckResultExtended(new HealthCheckResult(HealthStatus.Unhealthy));

            Assert.Equal(ExpectedResponse, ActResponse);
        }


        [Fact(DisplayName = "Should throw error if specific HC not founded")]
        public void ShouldReturnNullIfSearchedCheckNotFound()
        {
            var customBuilder = new HealthChecksBuilder();
            customBuilder.HealthChecks = new Dictionary<string, IHealthCheck>() {
                {"AnyImplementation", mockDegradedHC.Object },
                {"AnotherImplementation", mockThrowsHC.Object }
            };

            var sut = new HealthCheckService(customBuilder);

            Func<Task> act = () => sut.GetHealthAsync("DoenstExistImplementation");

            Assert.ThrowsAsync<OperationCanceledException>(act);
        }

    }
}
