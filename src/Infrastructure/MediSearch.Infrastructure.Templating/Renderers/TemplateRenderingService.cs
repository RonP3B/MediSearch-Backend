using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Templating.Renderers.Html;

namespace MediSearch.Infrastructure.Templating.Renderers;

internal sealed class TemplateRenderingService(IHtmlTemplateRenderer htmlTemplateRenderer)
    : ITemplateRenderingService
{
    private readonly IHtmlTemplateRenderer _htmlTemplateRenderer = htmlTemplateRenderer;

    public Task<string> RenderHtmlAsync<TModel>(
        TModel model,
        CancellationToken cancellationToken = default
    )
    {
        return _htmlTemplateRenderer.RenderAsync(model, cancellationToken);
    }

    // Could add other rendering methods for different formats in the future if needed.
}
