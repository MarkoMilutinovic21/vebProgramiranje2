using Microsoft.ServiceFabric.Services.Runtime;

internal static class Program
{
    private static void Main()
    {
        ServiceRuntime.RegisterServiceAsync("EventDispatcherServiceType",
            context => new EventDispatcherService.EventDispatcherService(context)).GetAwaiter().GetResult();

        Thread.Sleep(Timeout.Infinite);
    }
}