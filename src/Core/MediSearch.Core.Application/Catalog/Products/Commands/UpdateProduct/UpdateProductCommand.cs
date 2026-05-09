using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Commands.UpdateProduct;

[Authorize(Permission = PermissionCodes.ModifyProduct)]
public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    Guid ClassificationId,
    IReadOnlyList<Guid> CategoryIds,
    double PriceAmount,
    string PriceCurrency,
    int Quantity,
    IReadOnlyList<FileDto> ImageFiles
) : ICommand<ProductDto>;
