using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    class AuthenticationService : IAuthenticationService
    {
        private HealthChecksBuilder _healthChecksBuilder { get; }
        public string ApiKey { get => _healthChecksBuilder.ApiKey; }
        public AuthenticationService(HealthChecksBuilder healthChecksBuilder)
        {
            _healthChecksBuilder = healthChecksBuilder;
        }

        public bool ValidateApiKey(HttpRequestMessage request)
        {
            //query.TryGetValue
            var query = request.RequestUri.ParseQueryString();
            string key = query["ApiKey"];
            return (key == ApiKey);
        }

        public bool NeedAuthentication()
        {
            if (String.IsNullOrWhiteSpace(ApiKey))
            {
                return false;
            }

            return true;
        }
    }
}
