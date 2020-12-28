using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AspNetStandard.Diagnostics.HealthChecks.Services
{
    interface IAuthenticationService
    {
        bool NeedAuthentication();
        bool ValidateApiKey(HttpRequestMessage request);
    }
}
