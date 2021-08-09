using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.Http
{
    public class HttpClientHealthCheck : IHealthCheck
    {
        static readonly HttpClient client = new HttpClient();
        private string _endpoint;

        public HttpClientHealthCheck(string endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(_endpoint);
                response.EnsureSuccessStatusCode();

                return new HealthCheckResult(HealthStatus.Healthy, "The Api is Healthy");

            }
            catch (HttpRequestException ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "The Api is Unhealthy", ex);

            }
        }
    }
}