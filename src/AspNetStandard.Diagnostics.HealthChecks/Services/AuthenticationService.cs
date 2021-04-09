using System;
using System.Net.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly string _apiKey;

        public AuthenticationService(string apiKey)
        {
            _apiKey = apiKey;           
        }

        public bool ValidateApiKey(HttpRequestMessage request)
        {

            //query.TryGetValue
            var query = request.RequestUri.ParseQueryString();
            string key = query["ApiKey"];
            return (key == _apiKey);
        }

        public bool NeedAuthentication()
        {
            return !string.IsNullOrWhiteSpace(_apiKey);
        }
    }
}