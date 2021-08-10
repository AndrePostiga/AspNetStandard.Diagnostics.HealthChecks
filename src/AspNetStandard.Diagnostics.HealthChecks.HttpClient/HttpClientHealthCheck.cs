using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Threading;
using System.Threading.Tasks;
using Http = System.Net.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpClient
{
    public class HttpClientHealthCheck : IHealthCheck
    {
        private readonly string _endpoint;
        private readonly Http.HttpClient client = new Http.HttpClient();

        public HttpClientHealthCheck(string endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Http.HttpResponseMessage response = await client.GetAsync(_endpoint);
                response.EnsureSuccessStatusCode();

                return new HealthCheckResult(HealthStatus.Healthy, "The Api is Healthy");
            }
            catch (Http.HttpRequestException ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "The Api is Unhealthy", ex);
            }
        }
    }
}