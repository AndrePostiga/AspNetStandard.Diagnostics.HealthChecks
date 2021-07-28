using System.ServiceModel.Activation;
using System.Web.Routing;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public static class WcfHeathCheckRouteExtension
    {
        public static WcfHealthCheckBuilder AddHealthChecks()
        {
            RouteTable.Routes.Add(new ServiceRoute("health/", new WebServiceHostFactory(),
              typeof(WcfHealthCheckService)));

            return new WcfHealthCheckBuilder();
        }
    }
}
