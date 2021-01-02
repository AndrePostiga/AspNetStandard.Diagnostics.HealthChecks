using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetStandard.Diagnostics.HealthChecks.Entities;
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
        Mock<IHealthCheck> mockHealthyHC = new Mock<IHealthCheck>();

        JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() },
        };

        public HealthCheckHandlerTests()
        {
            anyTokenInstance = new CancellationTokenSource().Token;
            mockHealthyHC.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "AnyDescription", null)));
            mockHcService.Setup(x => x.GetStatusCode(It.IsAny<HealthStatus>())).Returns(System.Net.HttpStatusCode.OK);

            var hcResponse = new HealthCheckResponse();            
            hcResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(
                Task.Run(() => mockHealthyHC.Object.CheckHealthAsync()).Result    
            ));
            mockHcService.Setup(x => x.GetHealthAsync(anyTokenInstance)).ReturnsAsync(hcResponse);

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
        }

        [Fact(DisplayName = "Should return error if hc was not found")]
        public async void ShouldReturnErroIfCheckWasNotFound()
        {
            var sut = new HealthCheckHandler(mockHcService.Object);

            var act = await sut.HandleRequest(httpMessageWithParameter, anyTokenInstance);

            mockHcService.Verify(x => x.GetHealthAsync("AnyOtherImplementation"), Times.Once());
            //mockHcService.Verify(x => x.GetHealthAsync(
            //        It.Is<string>(x => x == "AnyOtherImplementation")
            //    ),
            //    Times.Once()
            //);
        }

        //[Fact(DisplayName = "Should return correct response")]
        //public async void ShouldReturnCorrectResponse()
        //{
        //    var sut = new HealthCheckHandler(mockHcService.Object);

        //    var act = await sut.HandleRequest(httpMessage, anyTokenInstance);

        //    mockHcService.Verify(x => x.GetHealthAsync(anyTokenInstance), Times.Once());
        //    mockHcService.Verify(x => x.GetHealthAsync(
        //            It.Is<CancellationToken>(x => x == anyTokenInstance)
        //        ),
        //        Times.Once()
        //    );
        //}
    }
}
