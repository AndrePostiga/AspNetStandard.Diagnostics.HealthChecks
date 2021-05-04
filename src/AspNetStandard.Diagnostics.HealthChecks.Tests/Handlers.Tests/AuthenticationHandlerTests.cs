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
        private readonly Mock<IAuthenticationService> _mockAuthService = new Mock<IAuthenticationService>();
        private readonly Mock<IHandler> _mockNextHandler = new Mock<IHandler>();
        private readonly HealthChecksBuilder _hcBuilder = new HealthChecksBuilder();
        private readonly string _wrongApiKey;
        private readonly HttpRequestMessage _httpRequestFake;

        public AuthenticationHandlerTests()
        {
            _wrongApiKey = "123";
            _httpRequestFake = new HttpRequestMessage(HttpMethod.Get, "http://anyurl.com/?apikey=" + _wrongApiKey);

            _mockAuthService.Setup(x => x.ValidateApiKey(It.IsAny<string>())).Returns(false);
            _mockAuthService.Setup(x => x.ValidateApiKey("AnyApiKey")).Returns(true);
            _mockAuthService.Setup(x => x.NeedAuthentication()).Returns(true);
            _mockNextHandler.Setup(x => x.HandleRequest(_httpRequestFake, default)).ReturnsAsync(It.IsAny<HttpResponseMessage>());
        }

        [Fact(DisplayName = "Should return error if validation returns false")]
        public async Task ShouldReturnErrorIfValidationFails()
        {
            var hcCustomBuilder = new HealthChecksBuilder().UseAuthorization("AnotherApiKey");

            var sut = new AuthenticationHandler(hcCustomBuilder.HealthCheckConfig, _mockAuthService.Object);
            var act = await sut.HandleRequest(_httpRequestFake, default);

            string json = JsonConvert.SerializeObject(new ForbiddenError().HttpErrorResponse, hcCustomBuilder.HealthCheckConfig.SerializerSettings);

            Assert.Equal(HttpStatusCode.Forbidden, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());
        }

        [Fact(DisplayName = "Should call validate apikey with correct api key")]
        public async Task ShouldCallValidateWithCorrectArgument()
        {
            var sut = new AuthenticationHandler(_hcBuilder.HealthCheckConfig, _mockAuthService.Object);

            await sut.HandleRequest(_httpRequestFake, default);
            _mockAuthService.Verify(x =>
                    x.ValidateApiKey(It.Is<string>(p => string.Equals(p, _wrongApiKey))
                ),
                Times.Once()
            );
        }

        [Fact(DisplayName = "Should call next handler with correct arguments if no authentication is needed")]
        public async Task ShouldSetNextHandlerCorrectly()
        {
            _mockAuthService.Setup(x => x.NeedAuthentication()).Returns(false);

            var sut = new AuthenticationHandler(_hcBuilder.HealthCheckConfig, _mockAuthService.Object);

            var actNextHandler = sut.SetNextHandler(_mockNextHandler.Object);

            var cancellationToken = new CancellationTokenSource().Token;

            await sut.HandleRequest(_httpRequestFake, cancellationToken);

            Assert.True(actNextHandler != null);
            _mockNextHandler.Verify(x => x.HandleRequest(_httpRequestFake, cancellationToken), Times.Once());
            _mockNextHandler.Verify(x => x.HandleRequest(
                    It.Is<HttpRequestMessage>(y => Object.ReferenceEquals(y, _httpRequestFake)),
                    It.Is<CancellationToken>(p => p == cancellationToken)
                ),
                Times.Once()
            );
        }


        [Fact(DisplayName = "Should call nextHandler with correct parameters if validate pass")]
        public async Task ShouldCallNextHandlerWithCorrectParameters()
        {
            var httpRequestWithCorrectApiKey = new HttpRequestMessage(HttpMethod.Get, "http://anyurl.com/?apikey=AnyApiKey");

            var sut = new AuthenticationHandler(_hcBuilder.HealthCheckConfig, _mockAuthService.Object);

            var actNextHandler = sut.SetNextHandler(_mockNextHandler.Object);

            var cancellationToken = new CancellationTokenSource().Token;

            await sut.HandleRequest(httpRequestWithCorrectApiKey, cancellationToken);

            Assert.True(actNextHandler != null);
            _mockNextHandler.Verify(m => m.HandleRequest(httpRequestWithCorrectApiKey, cancellationToken), Times.Once());
            _mockNextHandler.Verify(x => x.HandleRequest(
                    It.Is<HttpRequestMessage>(p => Object.ReferenceEquals(p, httpRequestWithCorrectApiKey)),
                    It.Is<CancellationToken>(y => y == cancellationToken)
                ),
                Times.Once()
            );
        }
    }
}