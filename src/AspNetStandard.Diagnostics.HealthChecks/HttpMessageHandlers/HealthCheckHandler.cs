using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Errors;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal class HealthCheckHandler : Handler
    {
        private readonly IHealthCheckService _hcService;

        public HealthCheckHandler(IHealthCheckService healthCheckService)
        {
            _hcService = healthCheckService;
        }

        public async override Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var queryParameters = request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            if (queryParameters.TryGetValue("check", out var check) && !string.IsNullOrWhiteSpace(check))
            {
                var healthResult = await _hcService.GetHealthAsync(check);

                if (healthResult == null)
                {
                    var error = new NotFoundError(check);
                    return MakeResponse<Object>(error.HttpErrorResponse, error.HttpErrorStatusCode);
                }

                return MakeResponse<HealthCheckResultExtended>(healthResult, _hcService.GetStatusCode(healthResult.Status));
            }

            var result = await _hcService.GetHealthAsync(cancellationToken);
            return MakeResponse<HealthCheckResponse>(result, _hcService.GetStatusCode(result.OverAllStatus));
        }
    }
}