using AspNetStandard.Diagnostics.HealthChecks.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal class AuthenticationHandler : BaseHandler, IChainable
    {
        public AuthenticationHandler(HttpConfiguration httpConfiguration, HealthChecksBuilder healthChecksBuilder) : base(httpConfiguration, healthChecksBuilder)
        { }

        #region IChainable Implementation

        private IHandler _nextHandler;

        public IHandler SetNextHandler(IHandler nextHandlerInstance)
        {
            _nextHandler = nextHandlerInstance;
            return nextHandlerInstance;
        }

        #endregion IChainable Implementation

        #region BaseHandler Implementation

        public async override Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(HealthChecksBuilder.ApiKey))
            {
                return await _nextHandler.HandleRequest(request, cancellationToken);
            }

            if (!validateKey(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new ObjectContent<ErrorResponse>(
                        new ErrorResponse { Error = $"ApiKey is invalid or not provided." },
                        new JsonMediaTypeFormatter { SerializerSettings = SerializerSettings }
                    )
                };

                return response;
            }

            return await _nextHandler.HandleRequest(request, cancellationToken);
        }

        #endregion BaseHandler Implementation

        #region Private Help Methods

        private bool validateKey(HttpRequestMessage request)
        {
            //query.TryGetValue
            var query = request.RequestUri.ParseQueryString();
            string key = query["ApiKey"];
            return (key == HealthChecksBuilder.ApiKey);
        }

        #endregion Private Help Methods
    }
}