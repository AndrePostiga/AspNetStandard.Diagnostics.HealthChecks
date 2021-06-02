using AspNetStandard.Diagnostics.HealthChecks.Errors;
using AspNetStandard.Diagnostics.HealthChecks.Seedwork;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal class AuthenticationHandler : Handler, IChainable
    {
        private IHandler _nextHandler;
        private readonly IAuthenticationService _authService;
        private readonly IHealthCheckConfiguration _hcConfig;

        public AuthenticationHandler(IHealthCheckConfiguration healthCheckConfiguration, IAuthenticationService service) : base(healthCheckConfiguration)
        {
            _hcConfig = healthCheckConfiguration;
            _authService = service;
        }

        public IHandler SetNextHandler(IHandler nextHandlerInstance)
        {
            _nextHandler = nextHandlerInstance;
            return nextHandlerInstance;
        }

        public override async Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var queryParameters = request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            queryParameters.TryGetValue("apikey", out var apiKey);

            if (!_authService.NeedAuthentication())
            {
                return await _nextHandler.HandleRequest(request, cancellationToken);
            }

            if (!_authService.ValidateApiKey(apiKey))
            {
                var error = new ForbiddenError(apiKey);
                _hcConfig.Logger?.LogException(error, _hcConfig.LoggerProperties);
                return MakeResponse(error.HttpErrorResponse, error.HttpErrorStatusCode);
            }

            return await _nextHandler.HandleRequest(request, cancellationToken);
        }
    }
}