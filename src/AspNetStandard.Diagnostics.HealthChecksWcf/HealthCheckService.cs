using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.SqlServer;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Configuration;
using System.Collections.Generic;
using AspNetStandard.Diagnostics.HealthChecks;
using System;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class HealthCheckService
    {
        public HealthCheckService()
        {

        }

        [OperationContract]
        [WebInvoke(
           Method = "GET",
           UriTemplate = "",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json)]
        public HealthCheckResponse ExecuteHealthCheck()
        {
            var healthCheckResponse = new HealthCheckResponse();

            var dependencies = HealthCheckWcfConfiguration.HealthChecksDependencies;
            foreach (var dependency in dependencies ) 
            {
                var result = dependency.Value.CheckHealthAsync().Result;

               // healthCheckResponse.Status = result.Status.ToString();
                
                healthCheckResponse.HealthChecks
                  .Add(dependency.Key, new HealthCheckResultExtended(result));
            }

            return healthCheckResponse;
        }
    }
}
