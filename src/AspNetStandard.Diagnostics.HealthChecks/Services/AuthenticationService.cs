namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IHealthCheckConfiguration _hcConfig;

        public AuthenticationService(IHealthCheckConfiguration hcConfig)
        {
            _hcConfig = hcConfig;
        }

        public bool ValidateApiKey(string apiKeyRequest)
        {
            return apiKeyRequest == _hcConfig.ApiKey;
        }

        public bool NeedAuthentication()
        {
            return !string.IsNullOrWhiteSpace(_hcConfig.ApiKey);
        }
    }
}