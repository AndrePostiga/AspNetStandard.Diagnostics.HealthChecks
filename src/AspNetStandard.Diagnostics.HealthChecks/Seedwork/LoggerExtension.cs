using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System;

namespace AspNetStandard.Diagnostics.HealthChecks.Seedwork
{
    public static class LoggerExtension
    {
        private static readonly string _messageTemplate = "[{Application}: HealthCheck] ";

        public static void LogHealthCheck(this ILogger logger, object content, HealthStatus status, string message = "")
        {
            LogContext.PushProperty("Content", content, true);
            var typeLog = LogEventLevel.Information;
            if (status.Equals(HealthStatus.Unhealthy))
                typeLog = LogEventLevel.Error;

            logger.Write(typeLog, _messageTemplate + message + " - " + status);
        }

        public static void LogException(this ILogger logger, Exception error)
        {
            logger.Error(error, _messageTemplate);
        }
    }
}
