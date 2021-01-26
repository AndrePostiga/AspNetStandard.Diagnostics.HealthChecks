using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public static class HttpConfigurationExtensions
    {
        public static HealthChecksBuilder AddHealthChecks(this HttpConfiguration httpConfiguration, string healthEndpoint = "health")
        {
            System.Diagnostics.Debug.WriteLine("Iniciei"); // ToDo: Era para isso estar aqui mesmo?
            var healthChecksBuilder = new HealthChecksBuilder();

            var healthChecksService = new HealthCheckService(healthChecksBuilder);
            var authenticationService = new AuthenticationService(healthChecksBuilder);

            var dependencyHandler = new DependencyHandler(httpConfiguration, healthChecksBuilder);
            var authenticationHandler = new AuthenticationHandler(authenticationService);
            var healthCheckHandler = new HealthCheckHandler(healthChecksService);

            dependencyHandler.SetNextHandler(authenticationHandler);
            authenticationHandler.SetNextHandler(healthCheckHandler);

            httpConfiguration.Routes.MapHttpRoute(
                name: "health_check",
                routeTemplate: healthEndpoint,
                defaults: new { check = RouteParameter.Optional },
                constraints: null,
                handler: dependencyHandler
            );

            return healthChecksBuilder;
        }
    }
}