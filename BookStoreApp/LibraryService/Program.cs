using Microsoft.ServiceFabric.Services.Runtime;

internal static class Program
{
    private static void Main()
    {
        ServiceRuntime.RegisterServiceAsync("LibraryServiceType",
            context => new LibraryService.LibraryService(context)).GetAwaiter().GetResult();

        Thread.Sleep(Timeout.Infinite);
    }
}