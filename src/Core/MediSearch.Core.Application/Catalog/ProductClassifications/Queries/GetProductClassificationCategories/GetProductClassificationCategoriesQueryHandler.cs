using MediSearch.Core.Application.Catalog.ProductClassifications.Constants;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.Ports;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Queries.GetProductClassificationCategories;

public sealed class GetProductClassificationCategoriesQueryHandler(
    IProductClassificationQueryService productClassificationQueryService,
    ICacheService cacheService
) : IQueryHandler<GetProductClassificationCategoriesQuery, IReadOnlyList<ClassificationCategoryDto>>
{
    private readonly IProductClassificationQueryService _productClassificationQueryService =
        productClassificationQueryService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<IReadOnlyList<ClassificationCategoryDto>> Handle(
        GetProductClassificationCategoriesQuery query,
        CancellationToken cancellationToken
    )
    {
        string cacheKey = ProductClassificationCacheKeys.ClassificationCategories(
            query.ClassificationId
        );

        var cached = await _cacheService.GetAsync<IReadOnlyList<ClassificationCategoryDto>>(
            cacheKey,
            cancellationToken
        );

        if (cached is not null)
        {
            return cached;
        }

        var result =
            await _productClassificationQueryService.GetProductClassificationCategoriesAsync(
                query.ClassificationId,
                cancellationToken
            );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(12), cancellationToken);

        return result;
    }
}
