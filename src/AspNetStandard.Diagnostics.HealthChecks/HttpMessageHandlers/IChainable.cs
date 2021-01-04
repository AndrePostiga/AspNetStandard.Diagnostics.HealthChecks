namespace AspNetStandard.Diagnostics.HealthChecks.HttpMessageHandlers
{
    public interface IChainable
    {
        IHandler SetNextHandler(IHandler nextHandlerInstance);
    }
}