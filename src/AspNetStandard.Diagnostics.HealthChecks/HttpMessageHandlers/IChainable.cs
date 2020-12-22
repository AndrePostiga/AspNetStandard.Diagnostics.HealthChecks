using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    interface IChainable
    {
        IHandler SetNextHandler(IHandler nextHandlerInstance);
    }
}
