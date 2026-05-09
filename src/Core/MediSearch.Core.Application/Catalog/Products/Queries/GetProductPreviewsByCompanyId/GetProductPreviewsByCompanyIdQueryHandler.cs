using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Ports;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductPreviewsByCompanyId;

public sealed class GetProductPreviewsByCompanyIdQueryHandler(
    IProductQueryService productQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetProductPreviewsByCompanyIdQuery, IReadOnlyList<ProductPreviewDto>>
{
    private readonly IProductQueryService _productQueryService = productQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<ProductPreviewDto>> Handle(
        GetProductPreviewsByCompanyIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var currentAgent = _currentUser.ToAgentOrNull();

        return await _productQueryService.GetProductPreviewsByCompanyIdAsync(
            query.CompanyId,
            currentAgent?.AgentId,
            currentAgent?.AgentTypeId,
            cancellationToken
        );
    }
}
