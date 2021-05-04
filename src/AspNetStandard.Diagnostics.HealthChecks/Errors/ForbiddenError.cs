using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks.Errors
{
    internal class ForbiddenError : HttpError
    {
        public ForbiddenError(string apiKey) : base($@"ApiKey {apiKey} is invalid or not provided.", HttpStatusCode.Forbidden)
        {
        }
    }
}