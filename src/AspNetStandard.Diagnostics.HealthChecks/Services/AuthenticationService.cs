using System;
using System.Net.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IHealthCheckConfiguration _hcConfig;

        public AuthenticationService(IHealthCheckConfiguration hcConfig)
        {
            _hcConfig = hcConfig;
        }

        public bool ValidateApiKey(string apiKeyrequest)
        {
            return apiKeyrequest == _hcConfig.ApiKey;
        }

        public bool NeedAuthentication()
        {
            return !string.IsNullOrWhiteSpace(_hcConfig.ApiKey);
        }
    }
}