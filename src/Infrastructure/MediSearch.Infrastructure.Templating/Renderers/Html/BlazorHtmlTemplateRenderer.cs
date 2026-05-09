using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediSearch.Infrastructure.Templating.Renderers.Html;

internal sealed class BlazorHtmlTemplateRenderer(IServiceProvider serviceProvider)
    : IHtmlTemplateRenderer
{
    private const string ModelParameterName = "Model";

    private static readonly Lazy<IReadOnlyDictionary<string, Type>> TemplateIndex = new(
        BuildTemplateIndex
    );

    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<string> RenderAsync<TModel>(
        TModel model,
        CancellationToken cancelationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(model);

        cancelationToken.ThrowIfCancellationRequested();

        var templateType = ResolveTemplateType(model.GetType());

        await using var scope = _serviceProvider.CreateAsyncScope();

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

        await using var renderer = new HtmlRenderer(scope.ServiceProvider, loggerFactory);

        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            cancelationToken.ThrowIfCancellationRequested();

            var parameters = ParameterView.FromDictionary(
                new Dictionary<string, object?> { [ModelParameterName] = model }
            );

            var output = await renderer.RenderComponentAsync(templateType, parameters);

            return output.ToHtmlString();
        });
    }

    private static Type ResolveTemplateType(Type modelType)
    {
        var templateName = GetTemplateName(modelType);

        if (!TemplateIndex.Value.TryGetValue(templateName, out var templateType))
        {
            throw new InvalidOperationException(
                $"HTML template component '{templateName}.razor' was not found for model '{modelType.Name}'."
            );
        }

        return templateType;
    }

    private static string GetTemplateName(Type modelType)
    {
        var typeName = modelType.Name;

        return typeName.EndsWith("Model", StringComparison.Ordinal)
            ? typeName[..^"Model".Length]
            : typeName;
    }

    private static IReadOnlyDictionary<string, Type> BuildTemplateIndex()
    {
        var templateTypes = typeof(BlazorHtmlTemplateRenderer)
            .Assembly.GetTypes()
            .Where(t =>
                t is { IsAbstract: false, IsClass: true, Namespace: not null }
                && typeof(IComponent).IsAssignableFrom(t)
                && t.Namespace.EndsWith(".Templates.Html", StringComparison.Ordinal)
                && t.GetProperty(ModelParameterName) is not null
            )
            .ToArray();

        var duplicatedTemplateNames = templateTypes
            .GroupBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (duplicatedTemplateNames.Length > 0)
        {
            throw new InvalidOperationException(
                $"Duplicate HTML template components were found: {string.Join(", ", duplicatedTemplateNames)}."
            );
        }

        return templateTypes.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
    }
}
