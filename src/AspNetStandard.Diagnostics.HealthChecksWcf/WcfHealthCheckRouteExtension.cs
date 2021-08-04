using System.ServiceModel.Activation;
using System.Web.Routing;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public static class WcfHealthCheckRouteExtension

    {
        public static WcfHealthCheckBuilder AddWcfHealthCheck(string healthEndpoint = "health/")
        {
            RouteTable.Routes.Add(new ServiceRoute(healthEndpoint, new WebServiceHostFactory(),
              typeof(WcfHealthCheckService)));

            return new WcfHealthCheckBuilder();
        }
    }
}
