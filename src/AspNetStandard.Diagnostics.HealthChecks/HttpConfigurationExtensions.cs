using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System.Collections.Generic;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks
{
    public static class HttpConfigurationExtensions
    {
        public static HealthChecksBuilder AddHealthChecks(this HttpConfiguration httpConfiguration, string healthEndpoint = "health")
        {
            var hcBuilder = new HealthChecksBuilder();
            var hcConfig = hcBuilder.Build();
            var dependencyResolver = httpConfiguration.DependencyResolver;

            

            var healthChecksService = new HealthCheckService(dependencyResolver, hcConfig.HealthChecksDependencies);
            var authenticationService = new AuthenticationService(hcConfig.ApiKey);
            var authenticationHandler = new AuthenticationHandler(hcConfig, authenticationService);
            var healthCheckHandler = new HealthCheckHandler(hcConfig, healthChecksService);
            authenticationHandler.SetNextHandler(healthCheckHandler);

            httpConfiguration.Routes.MapHttpRoute(
                name: "health_check",
                routeTemplate: healthEndpoint,
                defaults: new { check = RouteParameter.Optional },
                constraints: null,
                handler: authenticationHandler
            );

            return hcBuilder;
        }
    }
}