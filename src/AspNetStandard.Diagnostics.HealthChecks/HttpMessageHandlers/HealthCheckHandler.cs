using AspNetStandard.Diagnostics.HealthChecks.Entities;
using AspNetStandard.Diagnostics.HealthChecks.Models;
using AspNetStandard.Diagnostics.HealthChecks.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal class HealthCheckHandler : BaseHandler
    {
        public HealthCheckHandler(HttpConfiguration httpConfiguration, HealthChecksBuilder healthChecksBuilder) : base(httpConfiguration, healthChecksBuilder)
        {
        }

        #region BaseHandler Implementation

        public async override Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var healthChecks = GetHealthChecks();
            var service = new HealthCheckService(healthChecks, HealthChecksBuilder.ResultStatusCodes);

            var queryParameters = request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            if (queryParameters.TryGetValue("check", out var check) && !string.IsNullOrEmpty(check))
            {
                var healthResult = await service.GetHealthAsync(check);

                if (healthResult == null)
                {
                    return CheckNotFound(check);
                }

                return GetResponse(healthResult, healthResult.Status, service);
            }

            var result = await service.GetHealthAsync(cancellationToken);

            return GetResponse(result, result.OverAllStatus, service);
        }

        #endregion BaseHandler Implementation

        #region Private Help Methods

        private HttpResponseMessage GetResponse<T>(T objectContent, HealthStatus healthStatus, HealthCheckService healthCheckService)
        {
            var response = new HttpResponseMessage(healthCheckService.GetStatusCode(healthStatus))
            {
                Content = new ObjectContent<T>(objectContent, new JsonMediaTypeFormatter { SerializerSettings = SerializerSettings })
            };

            AddWarningHeaderIfNeeded(response, healthStatus);

            return response;
        }

        private IDictionary<string, IHealthCheck> GetHealthChecks()
        {
            using (var dependencyScope = _httpConfiguration.DependencyResolver.BeginScope())
            {
                var result = new Dictionary<string, IHealthCheck>();

                foreach (var registration in HealthChecksBuilder.HealthChecks)
                {
                    if (registration.Value.IsSingleton)
                    {
                        result.Add(registration.Key, registration.Value.Instance);
                    }
                    else
                    {
                        var instance = (IHealthCheck)dependencyScope.GetService(registration.Value.Type);

                        result.Add(registration.Key, instance);
                    }
                }

                return result;
            }
        }

        private HttpResponseMessage CheckNotFound(string check)
            => new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new ObjectContent<ErrorResponse>(
                    new ErrorResponse { Error = $"Health check '{check}' is not configured." },
                    new JsonMediaTypeFormatter { SerializerSettings = SerializerSettings })
            };

        private void AddWarningHeaderIfNeeded(HttpResponseMessage responseMessage, HealthStatus healthStatus)
        {
            if (HealthChecksBuilder.AddWarningHeader)
            {
                if (healthStatus == HealthStatus.Degraded)
                {
                    responseMessage.Headers.Warning.Add(new WarningHeaderValue(199, "health-check",
                        "\"Status is Degraded\""));
                }
                else if (healthStatus == HealthStatus.Unhealthy)
                {
                    responseMessage.Headers.Warning.Add(new WarningHeaderValue(199, "health-check",
                        "\"Status is Unhealthy\""));
                }
            }
        }

        #endregion Private Help Methods
    }
}