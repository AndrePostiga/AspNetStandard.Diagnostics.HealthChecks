using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Configuration;

namespace AspNetStandard.Diagnostics.HealthChecksWcf
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class HealthCheckService
    {
        private readonly IHealthCheck sqlHealthCheck;
        private const string sql = "select top 1 merchantId from merchant";

        public HealthCheckService()
        {
            this.sqlHealthCheck = new SqlServerHealthCheck(
                ConfigurationManager.ConnectionStrings["mundipaggDb"].ToString(),
                sql);
        }

        [OperationContract]
        [WebInvoke(
           Method = "GET",
           UriTemplate = "",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json)]
        public HealthCheckResponse ExecuteHealthCheck()
        {
            var result = sqlHealthCheck.CheckHealthAsync().Result;
            return new HealthCheckResponse
            {
                Status = result.Status.ToString()
            };
        }
    }
}
