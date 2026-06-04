using Microsoft.ServiceFabric.Services.Runtime;

internal static class Program
{
    private static void Main()
    {
        ServiceRuntime.RegisterServiceAsync("BackendSFType",
            context => new BackendSF.BackendSF(context)).GetAwaiter().GetResult();

        Thread.Sleep(Timeout.Infinite);
    }
}