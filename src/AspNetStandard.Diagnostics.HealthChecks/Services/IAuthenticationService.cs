using System.Net.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    public interface IAuthenticationService
    {
        bool NeedAuthentication();

        bool ValidateApiKey(HttpRequestMessage request);
    }
}