namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    internal interface IChainable
    {
        IHandler SetNextHandler(IHandler nextHandlerInstance);
    }
}