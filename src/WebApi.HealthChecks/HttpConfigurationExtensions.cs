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

            httpConfiguration.Routes.MapHttpRoute(
                name: "health_check",
                routeTemplate: healthEndpoint,
                defaults: new { check = RouteParameter.Optional },
                constraints: null,
                handler: new HealthHandler(httpConfiguration, healthChecksBuilder)
            );

            var ui = healthEndpoint + (healthEndpoint.EndsWith("/") ? string.Empty : "/") + "ui";

            httpConfiguration.Routes.MapHttpRoute(
                name: "health_check_ui",
                routeTemplate: ui,
                defaults: new { check = RouteParameter.Optional },
                constraints: null,
                handler: new HealthUiHandler(httpConfiguration, healthChecksBuilder)
            );

            return healthChecksBuilder;
        }
    }
}