using AspNetStandard.Diagnostics.HealthChecks.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    class AuthenticationHandler : BaseHandler, IChainable
    {
        
        public AuthenticationHandler(HttpConfiguration httpConfiguration, HealthChecksBuilder healthChecksBuilder) : base(httpConfiguration, healthChecksBuilder)
        {}

        #region IChainable Implementation
        private IHandler _nextHandler;
        public IHandler SetNextHandler(IHandler nextHandlerInstance)
        {
            _nextHandler = nextHandlerInstance;
            return nextHandlerInstance;
        }
        #endregion

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

                return await Task.FromResult(response); 
            }

            return await _nextHandler.HandleRequest(request, cancellationToken);
        }
        #endregion

        #region Private Help Methods
        private bool validateKey(HttpRequestMessage request)
        {
            var query = request.RequestUri.ParseQueryString();
            string key = query["ApiKey"];
            return (key == HealthChecksBuilder.ApiKey);
        }

        #endregion
    }
}
