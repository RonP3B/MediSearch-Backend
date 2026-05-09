using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Ports;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductPreviewsWithSeller;

public sealed class GetProductPreviewsWithSellerQueryHandler(
    IProductQueryService productQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetProductPreviewsWithSellerQuery, IReadOnlyList<ProductPreviewWithSellerDto>>
{
    private readonly IProductQueryService _productQueryService = productQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<ProductPreviewWithSellerDto>> Handle(
        GetProductPreviewsWithSellerQuery query,
        CancellationToken cancellationToken
    )
    {
        var currentAgent = _currentUser.ToAgentOrNull();

        return await _productQueryService.GetProductPreviewsWithSellerAsync(
            query.CompanyType,
            currentAgent?.AgentId,
            currentAgent?.AgentTypeId,
            cancellationToken
        );
    }
}
