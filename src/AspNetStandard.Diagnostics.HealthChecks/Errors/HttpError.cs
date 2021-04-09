using System;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks.Errors
{
    internal abstract class HttpError : Exception
    {
        public object HttpErrorResponse { get; }
        public HttpStatusCode HttpErrorStatusCode { get; }

        protected HttpError(string errorMessage, HttpStatusCode statusCode) : base(errorMessage)
        {
            HttpErrorResponse = new
            {
                statusCode = statusCode,
                message = errorMessage
            };

            HttpErrorStatusCode = statusCode;
        }
    }
}