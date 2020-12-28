using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks.Errors
{
    internal class ForbiddenError : HttpError
    {
        public ForbiddenError() : base($@"ApiKey is invalid or not provided.", HttpStatusCode.Forbidden)
        {
        }
    }
}