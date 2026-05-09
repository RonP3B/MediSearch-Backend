using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Commands.CreateProduct;

[Authorize(Permission = PermissionCodes.AddProduct)]
public sealed record CreateProductCommand(
    string Name,
    string Description,
    Guid ClassificationId,
    IReadOnlyList<Guid> CategoryIds,
    double PriceAmount,
    string PriceCurrency,
    int Quantity,
    IReadOnlyList<FileDto> ImageFiles
) : ICommand<ProductDto>;
