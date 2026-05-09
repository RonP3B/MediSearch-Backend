namespace Microsoft.Extensions.DependencyInjection;

internal static partial class DependencyInjection
{
    public static void AddExceptionHandlingServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
    }
}
