using System.Globalization;
using System.Reflection;
using Azure.Identity;
using Microsoft.AspNetCore.Localization;

namespace Microsoft.Extensions.DependencyInjection;

internal static partial class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        TypeAdapterConfig.GlobalSettings.Compile();

        builder.AddSignalRServices();

        builder.AddCorsServices();

        builder.AddOpenApiServices();

        builder.AddExceptionHandlingServices();

        builder.AddIdentityServices();

        builder.Services.Configure<RequestLocalizationOptions>(opts =>
        {
            var cultures = new[] { new CultureInfo("en"), new CultureInfo("es") };
            opts.DefaultRequestCulture = new RequestCulture("en");
            opts.SupportedCultures = cultures;
            opts.SupportedUICultures = cultures;
        });
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];

        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential()
            );
        }
    }
}
