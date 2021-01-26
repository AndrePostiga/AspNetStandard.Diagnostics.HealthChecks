using AspNetStandard.Diagnostics.HealthChecks.Errors;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal class AuthenticationHandler : Handler, IChainable
    {
        private IHandler _nextHandler;
        private readonly IAuthenticationService _authService;

        public AuthenticationHandler(IAuthenticationService service)
        {
            _authService = service;
        }

        public IHandler SetNextHandler(IHandler nextHandlerInstance)
        {
            _nextHandler = nextHandlerInstance;
            return nextHandlerInstance;
        }

        public override async Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken) // ToDo: Ajustei a ordem dos modificadores.
        {
            if (!_authService.NeedAuthentication())
            {
                return await _nextHandler.HandleRequest(request, cancellationToken);
            }

            if (!_authService.ValidateApiKey(request))
            {
                var error = new ForbiddenError();
                return MakeResponse(error.HttpErrorResponse, error.HttpErrorStatusCode); // ToDo: Removi o código não necessário.
            }

            return await _nextHandler.HandleRequest(request, cancellationToken);
        }
    }
}