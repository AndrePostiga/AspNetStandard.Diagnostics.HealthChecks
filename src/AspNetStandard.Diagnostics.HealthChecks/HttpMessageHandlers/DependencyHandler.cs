using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    public class DependencyHandler : Handler, IChainable
    {
        private readonly HttpConfiguration _httpConfig;
        private readonly HealthChecksBuilder _hcBuilder;
        private IHandler _nextHandler;

        public DependencyHandler(HttpConfiguration httpConfig, HealthChecksBuilder builder)
        {
            _httpConfig = httpConfig;
            _hcBuilder = builder;
        }

        public IHandler SetNextHandler(IHandler nextHandlerInstance)
        {
            _nextHandler = nextHandlerInstance;
            return nextHandlerInstance;
        }

        public async override Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_hcBuilder.HealthChecks != null)
            {
                System.Diagnostics.Debug.WriteLine("Não é nulo e vai direto");
                return await _nextHandler.HandleRequest(request, cancellationToken);
            }

            System.Diagnostics.Debug.WriteLine("Resolvendo as dependências");
            var result = new Dictionary<string, IHealthCheck>();

            var dependencyScope = _httpConfig.DependencyResolver.BeginScope();
            foreach (var registration in _hcBuilder.HealthChecksDependencies)
            {
                if (registration.Value.IsSingleton)
                {
                    result.Add(registration.Key, registration.Value.Instance);
                }
                else
                {
                    var instance = (IHealthCheck)dependencyScope.GetService(registration.Value.Type);
                    result.Add(registration.Key, instance);
                }
            }

            _hcBuilder.HealthChecks = new Dictionary<string, IHealthCheck>(result);
            return await _nextHandler.HandleRequest(request, cancellationToken);
        }
    }
}