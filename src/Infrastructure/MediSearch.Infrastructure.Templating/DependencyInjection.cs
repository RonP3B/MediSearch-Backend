using Ardalis.GuardClauses;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Templating.Renderers;
using MediSearch.Infrastructure.Templating.Renderers.Html;
using MediSearch.Infrastructure.Templating.Shared.UrlBuilder;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    private const string AppUrlsSectionKey = "AppUrls";

    public static void AddTemplatingServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<AppUrlOptions>(
            Guard.Against.Null(
                builder.Configuration.GetSection(AppUrlsSectionKey),
                $"Configuration key '{AppUrlsSectionKey}' is missing or empty."
            )
        );

        builder.Services.AddScoped<AppUrlBuilder>();
        builder.Services.AddSingleton<IHtmlTemplateRenderer, BlazorHtmlTemplateRenderer>();
        builder.Services.AddScoped<ITemplateRenderingService, TemplateRenderingService>();
    }
}
