using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<ProductDto>;
