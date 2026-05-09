using MediSearch.Core.Application.Catalog.ProductClassifications.Constants;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.Ports;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Queries.GetProductClassifications;

public sealed class GetProductClassificationsQueryHandler(
    IProductClassificationQueryService productClassificationQueryService,
    ICacheService cacheService
) : IQueryHandler<GetProductClassificationsQuery, IReadOnlyList<ProductClassificationDto>>
{
    private readonly IProductClassificationQueryService _productClassificationQueryService =
        productClassificationQueryService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<IReadOnlyList<ProductClassificationDto>> Handle(
        GetProductClassificationsQuery query,
        CancellationToken cancellationToken
    )
    {
        string cacheKey = ProductClassificationCacheKeys.AllProductClassifications;

        var cached = await _cacheService.GetAsync<IReadOnlyList<ProductClassificationDto>>(
            cacheKey,
            cancellationToken
        );

        if (cached is not null)
        {
            return cached;
        }

        var result = await _productClassificationQueryService.GetProductClassificationsAsync(
            cancellationToken
        );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(12), cancellationToken);

        return result;
    }
}
