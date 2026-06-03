using Microsoft.ServiceFabric.Services.Runtime;

internal static class Program
{
    private static void Main()
    {
        ServiceRuntime.RegisterServiceAsync("ValidatorType",
            context => new Validator.Validator(context)).GetAwaiter().GetResult();

        Thread.Sleep(Timeout.Infinite);
    }
}