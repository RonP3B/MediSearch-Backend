using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ProductDto> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetByIdOrDefaultAsync(
            EntityId<Product>.From(query.ProductId),
            cancellationToken
        );

        if (product is null)
        {
            throw NotFoundException.Entity(nameof(Product), nameof(Product.Id), query.ProductId);
        }

        return product.Adapt<ProductDto>();
    }
}
