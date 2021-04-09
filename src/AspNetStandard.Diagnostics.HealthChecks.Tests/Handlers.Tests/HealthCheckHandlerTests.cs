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
        private Mock<IHealthCheckService> _healthCheckServiceMock = new Mock<IHealthCheckService>();
        private HttpRequestMessage httpMessageWithParameter;
        private HttpRequestMessage httpMessage;
        private Mock<IHealthCheck> _healthyHealthCheckMock = new Mock<IHealthCheck>();
        private HealthCheckResult _healthyHealthCheckResult = new HealthCheckResult(HealthStatus.Healthy, "AnyDescription", null);
        private HealthCheckResultExtended _healthyHealthCheckResultExtended;
        private HealthCheckConfiguration _hcConfiguration;

        public HealthCheckHandlerTests()
        {

            _healthyHealthCheckResultExtended = new HealthCheckResultExtended(_healthyHealthCheckResult) { ResponseTime = null, LastExecution = DateTime.MinValue }; ;
            _healthyHealthCheckMock.Setup(x => x.CheckHealthAsync(default)).Returns(Task.FromResult(_healthyHealthCheckResult));
            
            _hcConfiguration = new HealthChecksBuilder().Build();

            var hcResponse = new HealthCheckResponse();            
            hcResponse.Entries.Add("AnyImplementation", new HealthCheckResultExtended(
                Task.Run(() => _healthyHealthCheckMock.Object.CheckHealthAsync()).Result    
            ));            


            _healthCheckServiceMock
                .Setup(x => x.GetHealthAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(hcResponse));

            _healthCheckServiceMock
                .Setup(x => x.GetHealthAsync("AnyImplementation", It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_healthyHealthCheckResultExtended));

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
            var sut = new HealthCheckHandler(_hcConfiguration, _healthCheckServiceMock.Object);
            var act = await sut.HandleRequest(httpMessage, It.IsAny<CancellationToken>());
            _healthCheckServiceMock.Verify(x => x.GetHealthAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "Should call healthCheckAsync overload with correct parameters if query parameter is passed")]
        public async void ShouldCallWithCorrectParametersWithQueryParameter()
        {
            var sut = new HealthCheckHandler(_hcConfiguration, _healthCheckServiceMock.Object);
            var act = await sut.HandleRequest(httpMessageWithParameter, It.IsAny<CancellationToken>());

            _healthCheckServiceMock.Verify(x => x.GetHealthAsync("AnyImplementation", It.IsAny<CancellationToken>()), Times.Once());
            _healthCheckServiceMock.Verify(x => x.GetHealthAsync(It.Is<string>(x => x == "AnyImplementation"), It.IsAny<CancellationToken>()),Times.Once());

            string json = JsonConvert.SerializeObject(_healthyHealthCheckResultExtended, _hcConfiguration.SerializerSettings);

            Assert.Equal(HttpStatusCode.OK, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());
        }

        [Fact(DisplayName = "Should return not found error if hc throws")]
        public async Task ShouldReturnErroIfCheckWasNotFound()
        {
            _healthCheckServiceMock
                .Setup(x => x.GetHealthAsync("AnyImplementation", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundError("AnyImplementation"));

            var sut = new HealthCheckHandler(_hcConfiguration, _healthCheckServiceMock.Object);
            var act = await sut.HandleRequest(httpMessageWithParameter, It.IsAny<CancellationToken>());

            string json = JsonConvert.SerializeObject(new NotFoundError("AnyImplementation").HttpErrorResponse, _hcConfiguration.SerializerSettings);

            Assert.Equal(HttpStatusCode.NotFound, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());
        }
    }
}
