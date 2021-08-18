using System;
using System.Collections.Generic;
using AspNetStandard.Diagnostics.HealthChecksWcf.Entities;

namespace AspNetStandard.Diagnostics.HealthChecksWcf.WcfException
{
    public class WcfExceptionHandler
    {
        private readonly Exception _exception;

        public WcfExceptionHandler(Exception exception = null)
        {
           this._exception = exception;
        }

        public ExceptionWcf Handler() {
            if (this._exception == null)
                return null;
            var exp = new ExceptionWcf
            {
                Errors = new List<Error>()
                { new Error { source = this._exception.Source } },
                Message = this._exception.Message,
                InnerException = this._exception.InnerException,
                HelpLink = this._exception.HelpLink ,
                StackTraceString = this._exception.StackTrace,
                Type = this._exception.GetType().ToString()
            };
            return exp;    
        }

    }

}
