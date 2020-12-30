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
        HttpRequestMessage httpMessage;
        public AuthenticationServiceTests()
        {
            httpMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://anyDomain/health?ApiKey=AnyApiKey")
            };
        }

        [Fact(DisplayName ="Should return true when apiKey is correctly")]
        public void ShoudlReturnTrueOnValidation()
        {
            var builder = new HealthChecksBuilder().UseAuthorization("AnyApiKey");
            var sut = new AuthenticationService(builder);

            var act = sut.ValidateApiKey(httpMessage);

            Assert.True(act == true);
        }

        [Fact(DisplayName = "Should validate if authorization is needed")]
        public void ShoudlReturnTrueIfNeedValidation()
        {
            var builder = new HealthChecksBuilder().UseAuthorization("AnyApiKey");
            var sut = new AuthenticationService(builder);

            var act = sut.NeedAuthentication();

            Assert.True(act == true);
        }

        [Fact(DisplayName = "Should validate if authorization is not needed")]
        public void ShoudlReturnTrueIfNoNeedValidation()
        {
            var builder = new HealthChecksBuilder();
            var sut = new AuthenticationService(builder);

            var act = sut.NeedAuthentication();

            Assert.True(act == false);
        }
    }
}
