﻿using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecksWcf.Entities
{
    [DataContract]
    public class WcfHealthCheckResponse
    {
        public WcfHealthCheckResponse()
        {
            HealthChecks = new Dictionary<string, WcfHealthCheckResult>();
        }

        [DataMember]
        public string Status { get { return OverallStatus(); } set { Status = value; } }

        [DataMember]
        public IDictionary<string, WcfHealthCheckResult> HealthChecks { get; set; }

        private string OverallStatus()
        {
            if (HealthChecks.Values.Any(x => x.Status == HealthStatus.Unhealthy.ToString()))
            {
                return HealthStatus.Unhealthy.ToString();
            }

            if (HealthChecks.Values.Any(x => x.Status == HealthStatus.Degraded.ToString()))
            {
                return HealthStatus.Degraded.ToString();
            }

            return HealthStatus.Healthy.ToString();
        }

        public void GenerateWcfResult(string key, HealthCheckResult result)
        {
            HealthChecks.Add(key, new WcfHealthCheckResult() { Status = result.Status.ToString(), Description = result.Description, Exception = result.Exception });
        }
    }
}
