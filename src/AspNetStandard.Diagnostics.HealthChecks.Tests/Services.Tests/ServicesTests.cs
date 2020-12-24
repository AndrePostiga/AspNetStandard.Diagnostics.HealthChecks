using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AspNetStandard.Diagnostics.HealthChecks;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Threading.Tasks;
using System.Threading;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Services.Tests
{
    public class AnyImplementationOfIHealthcheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {            
            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "AnyDescription", null));
        }
    }
    public class AnyImplementationOfIHealthcheckThatIsDegraded : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Degraded, "AnyDescription", null));
        }
    }
    public class AnyImplementationOfIHealthcheckThatThrows : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            throw new TimeoutException();            
        }
    }

    public class ServicesTests
    {

        #region Before Test
        public HealthChecksBuilder getBuilder()
        {
            return new HealthChecksBuilder();
        }

        public Dictionary<string, IHealthCheck> getFakeHealthChecks()
        {
            var healthChecks = new Dictionary<string, IHealthCheck>()
            {
                { "AnyDatabase" , new AnyImplementationOfIHealthcheck() },
                { "AnyMessageQueue" , new AnyImplementationOfIHealthcheckThatIsDegraded() },
            };

            return healthChecks;
        }

        public Dictionary<string, IHealthCheck> getFakeHealthChecksThatThrows()
        {
            var healthChecks = new Dictionary<string, IHealthCheck>()
            {
                { "AnyDatabase" , new AnyImplementationOfIHealthcheckThatThrows() },
                { "AnyMessageQueue" , new AnyImplementationOfIHealthcheckThatThrows() },
            };

            return healthChecks;
        }

        internal HealthCheckService GetSut()
        {
            return new HealthCheckService(getFakeHealthChecks(), getBuilder().ResultStatusCodes);
        }
        #endregion


        [Fact(DisplayName = "Should return correct status code if builder remain not modified")]
        public void ShouldGetStandardStatusCode()
        {
            var sut = (HealthCheckService)GetSut();

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
            var customBuilder = getBuilder();
            customBuilder.OverrideResultStatusCodes(
                System.Net.HttpStatusCode.NotFound,
                System.Net.HttpStatusCode.InternalServerError,
                System.Net.HttpStatusCode.Created
            );

            var sut = new HealthCheckService(getFakeHealthChecks(), customBuilder.ResultStatusCodes);

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

            var sut = (HealthCheckService)GetSut();

            var ActHealthCheck = await sut.GetHealthAsync();

            var ExpectedEntries = new HealthCheckResponse();

            var entries = new Dictionary<string, HealthCheckResultExtended>()
            {
                { "AnyDatabase", new HealthCheckResultExtended(await getFakeHealthChecks()["AnyDatabase"].CheckHealthAsync()) },
                { "AnyMessageQueue", new HealthCheckResultExtended(await getFakeHealthChecks()["AnyMessageQueue"].CheckHealthAsync()) }
            };

            ExpectedEntries.Entries.Add("AnyDatabase", entries["AnyDatabase"]);
            ExpectedEntries.Entries.Add("AnyMessageQueue", entries["AnyMessageQueue"]);
            ExpectedEntries.OverAllStatus = HealthStatus.Degraded;
            
            Assert.Equal(ExpectedEntries.Entries["AnyDatabase"], ActHealthCheck.Entries["AnyDatabase"]);
            Assert.Equal(ExpectedEntries.Entries["AnyMessageQueue"], ActHealthCheck.Entries["AnyMessageQueue"]);
            Assert.Equal(ExpectedEntries.OverAllStatus, ActHealthCheck.OverAllStatus);
        }


        [Fact(DisplayName = "Should cancel check if cancelletion token is canceled")]
        public async Task ShouldCancelCheck()
        {

            var sut = (HealthCheckService)GetSut();

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Func<Task> act = () => sut.GetHealthAsync(token);            

            tokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(act);
        }      


        [Fact(DisplayName = "Should return unhealthy response if GetHealth throws exception")]
        public async Task ShouldReturnUnhealthIfThrows()
        {

            var sut = new HealthCheckService(getFakeHealthChecksThatThrows(), getBuilder().ResultStatusCodes);

            var act = await sut.GetHealthAsync();
            
            Assert.Equal(HealthStatus.Unhealthy, act.OverAllStatus);
        }


        [Fact(DisplayName = "Should return degraded status if at least one check is degraded")]
        public async Task ShouldReturnDegradedOverAll()
        {

            var sut = (HealthCheckService)GetSut();

            var act = await sut.GetHealthAsync();

            Assert.Equal(HealthStatus.Degraded, act.OverAllStatus);
        }



    }
}
