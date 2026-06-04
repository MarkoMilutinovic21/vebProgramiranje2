using Microsoft.ServiceFabric.Services.Runtime;

internal static class Program
{
    private static void Main()
    {
        ServiceRuntime.RegisterServiceAsync("BankServiceType",
            context => new BankService.BankService(context)).GetAwaiter().GetResult();

        Thread.Sleep(Timeout.Infinite);
    }
}