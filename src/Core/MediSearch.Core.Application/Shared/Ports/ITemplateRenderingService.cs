namespace MediSearch.Core.Application.Shared.Ports;

public interface ITemplateRenderingService
{
    Task<string> RenderHtmlAsync<TTemplateModel>(
        TTemplateModel templateModel,
        CancellationToken cancellationToken = default
    );
}
