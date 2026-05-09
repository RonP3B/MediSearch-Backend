namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddCachingServices();
        builder.AddCommunicationServices();
        builder.AddClockServices();
        builder.AddFileStorageServices();
        builder.AddLocalizationServices();
        builder.AddMessageBusServices();
        builder.AddPersistenceServices();
        builder.AddSecurityServices();
        builder.AddTemplatingServices();
    }
}
