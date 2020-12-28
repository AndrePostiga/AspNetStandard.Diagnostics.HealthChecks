using System.Net.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    internal interface IAuthenticationService
    {
        bool NeedAuthentication();

        bool ValidateApiKey(HttpRequestMessage request);
    }
}