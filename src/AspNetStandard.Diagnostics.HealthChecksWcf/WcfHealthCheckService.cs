using AspNetStandard.Diagnostics.HealthChecksWcf.Entities;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;


namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WcfHealthCheckService
    {
        public WcfHealthCheckService()
        {
        }

        [OperationContract]
        [WebInvoke(
           Method = "GET",
           UriTemplate = "")]
        public WcfHealthCheckResponse ExecuteHealthCheck()
        {
            var healthCheckResponse = new WcfHealthCheckResponse();

            var dependencies = WcfHealthCheckConfiguration.HealthChecksDependencies;
            foreach (var dependency in dependencies)
            {
                var result = dependency.Value.CheckHealthAsync().Result;

                healthCheckResponse.GenerateWcfResult(dependency.Key, result);
            }

            healthCheckResponse.OverAllStatus = healthCheckResponse.GetOverallStatus();
            return healthCheckResponse;
        }
    }
}
