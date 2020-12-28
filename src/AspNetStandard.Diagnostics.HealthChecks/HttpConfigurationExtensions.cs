using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public static class HttpConfigurationExtensions
    {
        public static HealthChecksBuilder AddHealthChecks(this HttpConfiguration httpConfiguration, string healthEndpoint = "health")
        {
            System.Diagnostics.Debug.WriteLine("Iniciei");
            var healthChecksBuilder = new HealthChecksBuilder();

            var healthChecksService = new HealthCheckService(httpConfiguration, healthChecksBuilder);
            var authenticationService = new AuthenticationService(healthChecksBuilder);

            var authenticationHandler = new AuthenticationHandler(authenticationService);
            var healthCheckHandler = new HealthCheckHandler(healthChecksService);
            authenticationHandler.SetNextHandler(healthCheckHandler);

            httpConfiguration.Routes.MapHttpRoute(
                name: "health_check",
                routeTemplate: healthEndpoint,
                defaults: new { check = RouteParameter.Optional },
                constraints: null,
                handler: authenticationHandler
            );

            return healthChecksBuilder;
        }
    }
}