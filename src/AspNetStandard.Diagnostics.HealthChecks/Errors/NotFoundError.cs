using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks.Errors
{
    internal class NotFoundError : HttpError
    {
        public NotFoundError(string healthCheckName) : base($@"Cannot found {healthCheckName} on registered HealthChecks", HttpStatusCode.NotFound)
        {
        }
    }
}