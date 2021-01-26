using System;
using System.Net;

namespace AspNetStandard.Diagnostics.HealthChecks.Errors
{
    internal class HttpError : Exception // ToDo: Esse cara serve só com uma classe base? Se sim, deveria ser abstrata e o construtor protected.
    {
        public object HttpErrorResponse { get; }
        public HttpStatusCode HttpErrorStatusCode { get; }

        public HttpError(string errorMessage, HttpStatusCode statusCode) : base(errorMessage)
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