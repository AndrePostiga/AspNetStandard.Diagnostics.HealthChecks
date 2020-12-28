using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks.Errors
{
    public class NotFoundError : HttpError
    {
        public NotFoundError(string healthCheckName) : base($@"Cannot found {healthCheckName} on registered HealthChecks", HttpStatusCode.NotFound)
        {
        }
    }
}