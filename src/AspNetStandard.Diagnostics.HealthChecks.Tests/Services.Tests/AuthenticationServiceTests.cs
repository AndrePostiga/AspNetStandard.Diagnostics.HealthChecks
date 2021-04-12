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
        private IHealthCheckConfiguration _hcConfig;

        public AuthenticationServiceTests()
        {
            _hcConfig = new HealthChecksBuilder()
                .UseAuthorization("AnyApiKey")
                .HealthCheckConfig;
        }

        [Fact(DisplayName ="Should return true when apiKey is correctly")]
        public void ShoudlReturnTrueOnValidation()
        {
            var sut = new AuthenticationService(_hcConfig);
            var act = sut.ValidateApiKey("AnyApiKey");
            Assert.True(act == true);
        }

        [Fact(DisplayName = "Should validate if authorization is needed")]
        public void ShoudlReturnTrueIfNeedValidation()
        {;
            var sut = new AuthenticationService(_hcConfig);
            var act = sut.NeedAuthentication();
            Assert.True(act == true);
        }

        [Fact(DisplayName = "Should validate if authorization is not needed")]
        public void ShoudlReturnTrueIfNoNeedValidation()
        {
            var customConfig = new HealthChecksBuilder()
                .HealthCheckConfig;

            var sut = new AuthenticationService(customConfig);
            var act = sut.NeedAuthentication();
            Assert.True(act == false);
        }
    }
}
