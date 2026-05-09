using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Ports;
using MediSearch.Core.Domain.Catalog.Products;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductDetailsById;

public sealed class GetProductDetailsByIdQueryHandler(IProductQueryService productQueryService)
    : IQueryHandler<GetProductDetailsByIdQuery, ProductDetailsDto>
{
    private readonly IProductQueryService _productQueryService = productQueryService;

    public async Task<ProductDetailsDto> Handle(
        GetProductDetailsByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await _productQueryService.GetProductDetailsByIdAsync(
            query.ProductId,
            cancellationToken
        );

        if (result is null)
        {
            throw NotFoundException.Entity(nameof(Product), nameof(Product.Id), query.ProductId);
        }

        return result;
    }
}
