using AspNetStandard.Diagnostics.HealthChecks.Entities;
using Serilog;
using Serilog.Context;
using Serilog.Core.Enrichers;
using Serilog.Events;
using System;
using System.Globalization;

namespace AspNetStandard.Diagnostics.HealthChecks.Seedwork
{
    public static class LoggerExtension
    {
        private static readonly string _messageTemplate = "[{Application}: HealthCheck]";

        public static void DefaultContextProperties()
        {
            LogContext.PushProperty("ExecutionKey", Guid.NewGuid(), true);
            LogContext.PushProperty("ExecutionTime", DateTime.Now.ToString("s", CultureInfo.InvariantCulture), true);
            LogContext.PushProperty("ExecutionTimeUTC", DateTime.UtcNow, true);
            LogContext.PushProperty("Operation", "HealthCheck", true);
        }

        public static void LogHealthCheck(this ILogger logger, object content, 
            HealthStatus status, PropertyEnricher[] properties, string message = "")
        {
            using(LogContext.Push(properties))
            {
                DefaultContextProperties();
                LogContext.PushProperty("MessageType", "CheckServices", true);
                LogContext.PushProperty("Content", content, true);
                var typeLog = LogEventLevel.Information;
                
                if (status.Equals(HealthStatus.Unhealthy))
                {
                    typeLog = LogEventLevel.Error;
                }                    

                logger.Write(typeLog, $"{_messageTemplate} {message} - {status}");
            }
        }

        public static void LogException(this ILogger logger, Exception error, PropertyEnricher[] properties)
        {
            using (LogContext.Push(properties))
            {
                DefaultContextProperties();
                LogContext.PushProperty("MessageType", "Error", true);
                logger.Error(error, $"{_messageTemplate} Error");
            }                
        }
    }
}
