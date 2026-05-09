using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductDetailsById;

public sealed record GetProductDetailsByIdQuery(Guid ProductId) : IQuery<ProductDetailsDto>;
