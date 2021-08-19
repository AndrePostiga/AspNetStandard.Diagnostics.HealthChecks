using System;
using System.Collections.Generic;
using AspNetStandard.Diagnostics.HealthChecks.Wfc.Entities;

namespace AspNetStandard.Diagnostics.HealthChecks.Wfc
{
    internal sealed class WcfExceptionHandler
    {
        private readonly Exception _exception;

        public WcfExceptionHandler(Exception exception = null)
        {
            _exception = exception;
        }

        public ExceptionWcf Handler()
        {
            if (_exception == null)
                return null;

            return new ExceptionWcf
            {
                Message = _exception.Message,
                InnerException = _exception.InnerException,
                HelpLink = _exception.HelpLink,
                StackTraceString = _exception.StackTrace,
                Type = _exception.GetType().ToString(),
                Errors = new List<Error>
                {
                    new Error
                    {
                        Source = _exception.Source
                    }
                }
            };
        }
    }
}
