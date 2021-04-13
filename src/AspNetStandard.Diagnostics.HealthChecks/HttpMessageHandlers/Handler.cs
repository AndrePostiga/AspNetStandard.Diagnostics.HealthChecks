using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal abstract class Handler : DelegatingHandler, IHandler
    {
        private readonly IHealthCheckConfiguration _hcConfig;

        protected Handler(IHealthCheckConfiguration hcConfig) => _hcConfig = hcConfig;

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
                Content = new ObjectContent<T>(objectContent, new JsonMediaTypeFormatter { SerializerSettings = _hcConfig.SerializerSettings})
            };

            return response;
        }

        public abstract Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}