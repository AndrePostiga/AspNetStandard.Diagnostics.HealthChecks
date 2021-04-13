using AspNetStandard.Diagnostics.HealthChecks.Errors;
using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Handlers.Tests
{
    public class AuthenticationHandlerTests
    {
        private Mock<IAuthenticationService> mockAuthservice = new Mock<IAuthenticationService>();
        private Mock<IHandler> mockNextHandler = new Mock<IHandler>();
        private HealthChecksBuilder _hcBuilder = new HealthChecksBuilder();
        private readonly string _wrongApiKey;
        private HttpRequestMessage _httpRequestFake;

        public AuthenticationHandlerTests()
        {
            _wrongApiKey = "123";
            _httpRequestFake = new HttpRequestMessage(HttpMethod.Get, "http://anyurl.com/?apikey=" + _wrongApiKey);

            mockAuthservice.Setup(x => x.ValidateApiKey(It.IsAny<string>())).Returns(false);
            mockAuthservice.Setup(x => x.ValidateApiKey("AnyApiKey")).Returns(true);
            mockAuthservice.Setup(x => x.NeedAuthentication()).Returns(true);
            mockNextHandler.Setup(x => x.HandleRequest(_httpRequestFake, default)).ReturnsAsync(It.IsAny<HttpResponseMessage>());
        }

        [Fact(DisplayName = "Should return error if validation returns false")]
        public async Task ShouldReturnErrorIfValidationFails()
        {
            var hcCustomBuilder = new HealthChecksBuilder().UseAuthorization("AnotherApiKey");

            var sut = new AuthenticationHandler(hcCustomBuilder.HealthCheckConfig, mockAuthservice.Object);            

            var act = await sut.HandleRequest(_httpRequestFake, default);

            string json = JsonConvert.SerializeObject(new ForbiddenError().HttpErrorResponse, hcCustomBuilder.HealthCheckConfig.SerializerSettings);

            Assert.Equal(HttpStatusCode.Forbidden, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());
        }
        

        [Fact(DisplayName = "Should call validate apikey with correct api key")]
        public async void ShouldCallValidateWithCorrectArgument()
        {
            var sut = new AuthenticationHandler(_hcBuilder.HealthCheckConfig, mockAuthservice.Object);

            var act = await sut.HandleRequest(_httpRequestFake, default);         

            mockAuthservice.Verify(x => 
                    x.ValidateApiKey(It.Is<string>(x => string.Equals(x, _wrongApiKey))
                ),
                Times.Once()
            );
        }

      
        [Fact(DisplayName = "Should call next handler with correct arguments if no authentication is needed")]
        public async void ShouldSetNextHandlerCorrectly()
        {
            mockAuthservice.Setup(x => x.NeedAuthentication()).Returns(false);

            var sut = new AuthenticationHandler(_hcBuilder.HealthCheckConfig, mockAuthservice.Object);

            var actNextHandler = sut.SetNextHandler(mockNextHandler.Object);

            var cancellationToken = new CancellationTokenSource().Token;

            var act = await sut.HandleRequest(_httpRequestFake, cancellationToken);

            Assert.True(actNextHandler != null);
            mockNextHandler.Verify(x => x.HandleRequest(_httpRequestFake, cancellationToken), Times.Once());
            mockNextHandler.Verify(x => x.HandleRequest(
                    It.Is<HttpRequestMessage>(x => Object.ReferenceEquals(x, _httpRequestFake)),
                    It.Is<CancellationToken>(x => x == cancellationToken)
                ),
                Times.Once()
            );

        }
       
        [Fact(DisplayName = "Should call nextHandler with correct parameters if validade pass")]
        public async void ShouldCallNextHandlerWithCorrectParameters()
        {           
            var httpRequestWithCorrectApiKey = new HttpRequestMessage(HttpMethod.Get, "http://anyurl.com/?apikey=AnyApiKey");            

            var sut = new AuthenticationHandler(_hcBuilder.HealthCheckConfig, mockAuthservice.Object);

            var actNextHandler = sut.SetNextHandler(mockNextHandler.Object);

            var cancellationToken = new CancellationTokenSource().Token;

            var act = await sut.HandleRequest(httpRequestWithCorrectApiKey, cancellationToken);

            Assert.True(actNextHandler != null);
            mockNextHandler.Verify(m => m.HandleRequest(httpRequestWithCorrectApiKey, cancellationToken), Times.Once());
            mockNextHandler.Verify(x => x.HandleRequest(
                    It.Is<HttpRequestMessage>(x => Object.ReferenceEquals(x, httpRequestWithCorrectApiKey)),
                    It.Is<CancellationToken>(x => x == cancellationToken)
                ),
                Times.Once()
            );
        }
      
    }
}
