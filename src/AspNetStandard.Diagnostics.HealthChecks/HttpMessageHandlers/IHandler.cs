using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    public interface IHandler
    {
        Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}