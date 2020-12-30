using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Net.Http;
using AspNetStandard.Diagnostics.HealthChecks.Errors;
using System.Net;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Handlers.Tests
{
    public class AuthenticationHandlerTests
    {
        Mock<IAuthenticationService> mockAuthservice = new Mock<IAuthenticationService>();
        JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() },
        };        

        public AuthenticationHandlerTests()
        {
            mockAuthservice.Setup(x => x.ValidateApiKey(new HttpRequestMessage())).Returns(false);
            mockAuthservice.Setup(x => x.NeedAuthentication()).Returns(true);
        }

        //[Fact(DisplayName = "Should set next handler correctly")]
        //public void ShouldSetNextHandlerCorrectly()
        //{   
        //}

        [Fact(DisplayName = "Should return error if validation returns false")]
        public async void ShouldReturnErrorIfValidationFails()
        {
            var sut = new AuthenticationHandler(mockAuthservice.Object);

            var act = await sut.HandleRequest(new HttpRequestMessage(), default);                       

            string json = JsonConvert.SerializeObject(new ForbiddenError().HttpErrorResponse, SerializerSettings);

            Assert.Equal(HttpStatusCode.Forbidden, act.StatusCode);
            Assert.Equal(json, await act.Content.ReadAsStringAsync());            
        }


    }
}
