using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System;
using Xunit;
using Moq;
using System.Net.Http;
using AspNetStandard.Diagnostics.HealthChecks.Errors;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Handlers.Tests
{
    public class AuthenticationHandlerTests
    {
        private Mock<IAuthenticationService> mockAuthservice = new Mock<IAuthenticationService>();
        private Mock<IHandler> mockNextHandler = new Mock<IHandler>();
        private HttpRequestMessage anyHttpRequestMessage;

        JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() },
        };        

        public AuthenticationHandlerTests()
        {

            anyHttpRequestMessage = It.IsAny<HttpRequestMessage>();

            mockAuthservice.Setup(x => x.ValidateApiKey(anyHttpRequestMessage)).Returns(false);
            mockAuthservice.Setup(x => x.NeedAuthentication()).Returns(true);
            mockNextHandler.Setup(x => x.HandleRequest(anyHttpRequestMessage, default)).ReturnsAsync(It.IsAny<HttpResponseMessage>());
        }        

        [Fact(DisplayName = "Should return error if validation returns false")]
        public async void ShouldReturnErrorIfValidationFails()
        {
            var sut = new AuthenticationHandler(mockAuthservice.Object);

            var act = await sut.HandleRequest(anyHttpRequestMessage, default);                       

            string json = JsonConvert.SerializeObject(new ForbiddenError().HttpErrorResponse, SerializerSettings);

            Assert.Equal(HttpStatusCode.Forbidden, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());            
        }

        [Fact(DisplayName = "Should call validate apikey with correct httpRequestMessage ")]
        public async void ShouldCallValidateWithCorrectArgument()
        {
            var sut = new AuthenticationHandler(mockAuthservice.Object);

            var act = await sut.HandleRequest(anyHttpRequestMessage, default);

            mockAuthservice.Verify(x => x.ValidateApiKey(
                    It.Is<HttpRequestMessage>(x => Object.ReferenceEquals(x, anyHttpRequestMessage))
                ), 
                Times.Once()
            );
        }

        [Fact(DisplayName = "Should call next handler with correct arguments if no authentication is needed")]
        public async void ShouldSetNextHandlerCorrectly()
        {
            mockAuthservice.Setup(x => x.NeedAuthentication()).Returns(false);

            var sut = new AuthenticationHandler(mockAuthservice.Object);

            var actNextHandler = sut.SetNextHandler(mockNextHandler.Object);

            var cancellationToken = new CancellationTokenSource().Token;

            var act = await sut.HandleRequest(anyHttpRequestMessage, cancellationToken);

            Assert.True(actNextHandler != null);
            mockNextHandler.Verify(x => x.HandleRequest(anyHttpRequestMessage, cancellationToken), Times.Once());
            mockNextHandler.Verify(x => x.HandleRequest(
                    It.Is<HttpRequestMessage>(x => Object.ReferenceEquals(x, anyHttpRequestMessage)),
                    It.Is<CancellationToken>(x => x == cancellationToken)
                ),
                Times.Once()
            );

        }

        [Fact(DisplayName = "Should call nextHandler with correct parameters if validade pass")]
        public async void ShouldCallNextHandlerWithCorrectParameters()
        {
            mockAuthservice.Setup(x => x.ValidateApiKey(anyHttpRequestMessage)).Returns(true);
            var sut = new AuthenticationHandler(mockAuthservice.Object);

            var actNextHandler = sut.SetNextHandler(mockNextHandler.Object);

            var cancellationToken = new CancellationTokenSource().Token;

            var act = await sut.HandleRequest(anyHttpRequestMessage, cancellationToken);

            Assert.True(actNextHandler != null);
            mockNextHandler.Verify(m => m.HandleRequest(anyHttpRequestMessage, cancellationToken), Times.Once());
            mockNextHandler.Verify(x => x.HandleRequest( 
                    It.Is<HttpRequestMessage>(x => Object.ReferenceEquals(x, anyHttpRequestMessage)), 
                    It.Is<CancellationToken>(x => x == cancellationToken)
                ), 
                Times.Once()
            );
        }
    }
}
