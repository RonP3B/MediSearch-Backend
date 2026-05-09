namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    public static void AddCommunicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddEmailServices();
    }
}
