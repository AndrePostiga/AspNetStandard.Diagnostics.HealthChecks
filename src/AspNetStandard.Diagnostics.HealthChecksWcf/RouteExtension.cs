using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    public static class RouteExtension
    {
        public static void AddHealthCheckRoute()
        {
            RouteTable.Routes.Add(new ServiceRoute("health/", new WebServiceHostFactory(),
                typeof(HealthCheckService)));
        }
    }
}
