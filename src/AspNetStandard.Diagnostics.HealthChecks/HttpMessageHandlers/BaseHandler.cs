using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal abstract class BaseHandler : DelegatingHandler, IHandler
    {
        protected JsonSerializerSettings SerializerSettings { get; } =
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        protected BaseHandler(HttpConfiguration httpConfiguration, HealthChecksBuilder healthChecksBuilder)
        {
            _httpConfiguration = httpConfiguration;
            HealthChecksBuilder = healthChecksBuilder;
        }

        public abstract Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken);

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
    }
}