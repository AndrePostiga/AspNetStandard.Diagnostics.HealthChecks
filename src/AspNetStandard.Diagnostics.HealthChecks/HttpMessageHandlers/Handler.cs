using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal abstract class Handler : DelegatingHandler, IHandler
    {
        protected JsonSerializerSettings SerializerSettings;
        
        protected Handler(HealthCheckConfiguration hcConfig) => SerializerSettings = hcConfig.SerializerSettings;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
            {
                throw new HttpRequestException("The method accepts only GET requests.");
            }

            return await HandleRequest(request, cancellationToken);
        }        

        protected HttpResponseMessage MakeResponse<T>(T objectContent, HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new ObjectContent<T>(objectContent, new JsonMediaTypeFormatter { SerializerSettings = SerializerSettings })
            };

            return response;
        }

        public abstract Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}