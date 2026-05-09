using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateProductClassification;

[Authorize(Permission = PermissionCodes.UpdateProductClassification)]
public sealed record UpdateProductClassificationCommand(Guid Id, string Name)
    : ICommand<ProductClassificationDto>;
