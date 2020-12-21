using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    interface IHasNextHandler
    {
        IHandler SetNextHandler(IHandler nextHandlerInstance);
    }
}
