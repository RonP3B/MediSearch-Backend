using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.CreateProductClassification;

[Authorize(Permission = PermissionCodes.CreateProductClassification)]
public sealed record CreateProductClassificationCommand(string Name)
    : ICommand<ProductClassificationDto>;
