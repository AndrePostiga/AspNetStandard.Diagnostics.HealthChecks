using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Errors;
using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Handlers.Tests
{
    public class HealthCheckHandlerTests
    {
        private Mock<IHealthCheckService> mockHcService = new Mock<IHealthCheckService>();
        private CancellationToken anyTokenInstance;
        private HttpRequestMessage httpMessageWithParameter;
        private HttpRequestMessage httpMessage;
        private Mock<IHealthCheck> mockHealthyHC = new Mock<IHealthCheck>();
        private HealthCheckResult HcResult;
        private HealthCheckResultExtended HcResultExtended;

        private JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() },
        };

        public HealthCheckHandlerTests()
        {
            anyTokenInstance = new CancellationTokenSource().Token;
            HcResult = new HealthCheckResult(HealthStatus.Healthy, "AnyDescription", null);
            HcResultExtended = new HealthCheckResultExtended(HcResult) { ResponseTime = null, LastExecution = DateTime.MinValue};
            mockHealthyHC.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(HcResult));
            

            var hcResponse = new HealthCheckResponse();            
            hcResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(
                Task.Run(() => mockHealthyHC.Object.CheckHealthAsync()).Result    
            ));

            mockHcService.Setup(x => x.GetStatusCode(It.IsAny<HealthStatus>())).Returns(HttpStatusCode.OK);
            mockHcService.Setup(x => x.GetHealthAsync(anyTokenInstance)).ReturnsAsync(hcResponse);            
            mockHcService.Setup(x => x.GetHealthAsync("AnyImplementation")).ReturnsAsync(HcResultExtended);            

            httpMessageWithParameter = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://anyDomain/health?check=AnyImplementation")
            };
            httpMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://anyDomain/health")
            };            
        }

        [Fact(DisplayName = "Should call healthCheckAsync with correct parameters")]
        public async void ShouldCallWithCorrectParameters()
        {   
            var sut = new HealthCheckHandler(mockHcService.Object);

            var act = await sut.HandleRequest(httpMessage, anyTokenInstance);

            mockHcService.Verify(x => x.GetHealthAsync(anyTokenInstance), Times.Once());
            mockHcService.Verify(x => x.GetHealthAsync(
                    It.Is<CancellationToken>(x => x == anyTokenInstance)
                ),
                Times.Once()
            );
        }

        [Fact(DisplayName = "Should call healthCheckAsync overload with correct parameters if query parameter is passed")]
        public async void ShouldCallWithCorrectParametersWithQueryParameter()
        {
            var sut = new HealthCheckHandler(mockHcService.Object);

            var act = await sut.HandleRequest(httpMessageWithParameter, anyTokenInstance);

            mockHcService.Verify(x => x.GetHealthAsync("AnyImplementation"), Times.Once());
            mockHcService.Verify(x => x.GetHealthAsync(
                    It.Is<string>(x => x == "AnyImplementation")
                ),
                Times.Once()
            );

            string json = JsonConvert.SerializeObject(HcResultExtended, SerializerSettings);

            Assert.Equal(HttpStatusCode.OK, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());
        }

        [Fact(DisplayName = "Should return not found error if hc throws")]
        public async Task ShouldReturnErroIfCheckWasNotFound()
        {
            mockHcService.Setup(x => x.GetHealthAsync("AnyImplementation")).ThrowsAsync(new NotFoundError("AnyImplementation"));

            var sut = new HealthCheckHandler(mockHcService.Object);

            var act = await sut.HandleRequest(httpMessageWithParameter, anyTokenInstance);            

            string json = JsonConvert.SerializeObject(new NotFoundError("AnyImplementation").HttpErrorResponse, SerializerSettings);            

            Assert.Equal(HttpStatusCode.NotFound, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());
        }
    }
}
