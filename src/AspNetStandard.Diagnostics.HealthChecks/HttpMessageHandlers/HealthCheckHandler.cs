using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Errors;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal class HealthCheckHandler : Handler
    {
        private readonly IHealthCheckService _hcService;
        private readonly IHealthCheckConfiguration _hcConfig;

        public HealthCheckHandler(IHealthCheckConfiguration healthCheckConfiguration, IHealthCheckService healthCheckService) : base(healthCheckConfiguration)
        {
            _hcService = healthCheckService;
            _hcConfig = healthCheckConfiguration;
        }

        public async override Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var queryParameters = request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            try
            {
                if (queryParameters.TryGetValue("check", out var check) && !string.IsNullOrWhiteSpace(check))
                {
                    var healthResult = await _hcService.GetHealthAsync(check, cancellationToken);
                    return MakeResponse<HealthCheckResultExtended>(healthResult, _hcConfig.GetStatusCode(healthResult.Status));
                }

                var result = await _hcService.GetHealthAsync(cancellationToken);
                return MakeResponse<HealthCheckResponse>(result, _hcConfig.GetStatusCode(result.OverAllStatus));
            }
            catch (NotFoundError error)
            {
                return MakeResponse(error.HttpErrorResponse, error.HttpErrorStatusCode);
            }            
        }
    }
}