using AspNetStandard.Diagnostics.HealthChecks.Services;
using Xunit;

namespace AspNetStandard.Diagnostics.HealthChecks.Tests.Services.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly IHealthCheckConfiguration _hcConfig;

        public AuthenticationServiceTests()
        {
            _hcConfig = new HealthChecksBuilder()
                .UseAuthorization("AnyApiKey")
                .HealthCheckConfig;
        }

        [Fact(DisplayName ="Should return true when apiKey is correctly")]
        public void ShouldReturnTrueOnValidation()
        {
            var sut = new AuthenticationService(_hcConfig);
            var act = sut.ValidateApiKey("AnyApiKey");
            Assert.True(act);
        }

        [Fact(DisplayName = "Should validate if authorization is needed")]
        public void ShouldReturnTrueIfNeedValidation()
        {
            var sut = new AuthenticationService(_hcConfig);
            var act = sut.NeedAuthentication();
            Assert.True(act);
        }

        [Fact(DisplayName = "Should validate if authorization is not needed")]
        public void ShouldReturnTrueIfNoNeedValidation()
        {
            var customConfig = new HealthChecksBuilder()
                .HealthCheckConfig;

            var sut = new AuthenticationService(customConfig);
            var act = sut.NeedAuthentication();
            Assert.False(act);
        }
    }
}
