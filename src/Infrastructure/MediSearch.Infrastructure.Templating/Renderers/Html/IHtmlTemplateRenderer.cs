namespace MediSearch.Infrastructure.Templating.Renderers.Html;

internal interface IHtmlTemplateRenderer
{
    Task<string> RenderAsync<TModel>(TModel model, CancellationToken cancelationToken);
}
