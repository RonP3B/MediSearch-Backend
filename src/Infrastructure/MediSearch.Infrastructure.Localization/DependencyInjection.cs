using System.Globalization;
using MediSearch.Infrastructure.Localization.Configuration.Culture;
using MediSearch.Infrastructure.Localization.Configuration.Json;
using MediSearch.Infrastructure.Localization.Configuration.Localizers.Error;
using MediSearch.Infrastructure.Localization.Configuration.Localizers.Text;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddLocalizationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<JsonLocalizationOptions>(options =>
        {
            options.ResourcesPaths =
            [
                .. Directory
                    .GetFiles(AppContext.BaseDirectory, "*.json", SearchOption.AllDirectories)
                    .Select(f => Path.GetDirectoryName(f))
                    .OfType<string>()
                    .Select(d => Path.GetRelativePath(AppContext.BaseDirectory, d))
                    .Distinct(),
            ];

            options.DefaultCulture = new CultureInfo("en");
            options.SupportedCultures = [new("en"), new("es")];
        });

        builder.Services.AddSingleton<JsonLocalizationStore>();
        builder.Services.AddScoped<ICultureProvider, HttpCultureProvider>();
        builder.Services.AddScoped<IErrorLocalizer, JsonErrorLocalizer>();
        builder.Services.AddScoped<ITextLocalizer, JsonTextLocalizer>();
    }
}
