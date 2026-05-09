using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateClassificationCategory;

[Authorize(Permission = PermissionCodes.UpdateClassificationCategory)]
public sealed record UpdateClassificationCategoryCommand(
    Guid ClassificationId,
    Guid CategoryId,
    string CategoryName
) : ICommand<ClassificationCategoryDto>;
