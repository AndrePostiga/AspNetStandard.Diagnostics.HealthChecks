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
    abstract class BaseHandler : DelegatingHandler, IHandler
    {        
        protected JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        protected BaseHandler(HttpConfiguration httpConfiguration, HealthChecksBuilder healthChecksBuilder)
        {
            _httpConfiguration = httpConfiguration;
            HealthChecksBuilder = healthChecksBuilder;
        }

        #region IHandler Implementation

        public abstract Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken);        

        #endregion

        #region DelegatingHandler Implementation
        protected HealthChecksBuilder HealthChecksBuilder { get; }

        protected readonly HttpConfiguration _httpConfiguration;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // only accepting GET
            if (request.Method != HttpMethod.Get)
            {
                throw new HttpRequestException("The method accepts only GET requests.");
            }

            //Problema está aqui, executa o send async assim que chama o handler
            return await HandleRequest(request, cancellationToken);            
        }

        #endregion
    }
}
