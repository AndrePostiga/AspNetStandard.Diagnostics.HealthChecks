using System.Web.Http;
using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public static class HttpConfigurationExtensions
    {
        public static HealthChecksBuilder AddHealthChecks(this HttpConfiguration httpConfiguration, string healthEndpoint = "health")
        {
            var healthChecksBuilder = new HealthChecksBuilder();
            var authenticationHandler = new AuthenticationHandler(httpConfiguration, healthChecksBuilder);
            var healthCheckHandler = new HealthCheckHandler(httpConfiguration, healthChecksBuilder);
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