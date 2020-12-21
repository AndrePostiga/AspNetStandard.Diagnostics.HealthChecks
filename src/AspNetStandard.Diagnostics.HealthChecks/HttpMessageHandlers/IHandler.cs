using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    interface IHandler
    {       
        Task<HttpResponseMessage> HandleRequest(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
