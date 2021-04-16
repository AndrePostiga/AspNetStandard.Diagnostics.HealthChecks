using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetStandard.Diagnostics.HealthChecks.Helpers
{
    public static class LogHelper
    {
        private static readonly string _messageTemplate = "[{ApplicationName}: HealthCheck] ";

        public static void LogHealthCheck(ILogger logger, object content, HealthStatus status, string message = "")
        {
            LogContext.PushProperty("Content", content, true);
            var typeLog = LogEventLevel.Information;
            if (status.Equals(HealthStatus.Unhealthy))
                typeLog = LogEventLevel.Error;

            logger.Write(typeLog, _messageTemplate + message + " - " + status);
        }

        public static void LogException(ILogger logger, Exception error)
        {
            logger.Error(error, _messageTemplate);
        }
    }
}
