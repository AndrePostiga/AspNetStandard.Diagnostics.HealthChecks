using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Net.Http;
using System.Collections.Specialized;
using AspNetStandard.Diagnostics.HealthChecks.Services;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Services.Tests
{
    public class AuthenticationServiceTests
    {
        HttpRequestMessage _httpMessage;
        string _apiKey;
        public AuthenticationServiceTests()
        {
            _httpMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://anyDomain/health?ApiKey=AnyApiKey")
            };

            _apiKey = "AnyApiKey";
        }

        [Fact(DisplayName ="Should return true when apiKey is correctly")]
        public void ShoudlReturnTrueOnValidation()
        {
            var sut = new AuthenticationService(_apiKey);
            var act = sut.ValidateApiKey(_httpMessage);
            Assert.True(act == true);
        }

        [Fact(DisplayName = "Should validate if authorization is needed")]
        public void ShoudlReturnTrueIfNeedValidation()
        {;
            var sut = new AuthenticationService(_apiKey);
            var act = sut.NeedAuthentication();
            Assert.True(act == true);
        }

        [Fact(DisplayName = "Should validate if authorization is not needed")]
        public void ShoudlReturnTrueIfNoNeedValidation()
        {
            var sut = new AuthenticationService(null);
            var act = sut.NeedAuthentication();
            Assert.True(act == false);
        }
    }
}
