using System.ServiceModel.Activation;
using System.Web.Routing;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public static class WcfHeathCheckRouteExtension
    {
        public static WcfHealthCheckBuilder AddWcfHealthCheck()
        {
            RouteTable.Routes.Add(new ServiceRoute("health/", new WebServiceHostFactory(),
              typeof(WcfHealthCheckService)));

            return new WcfHealthCheckBuilder();
        }
    }
}
