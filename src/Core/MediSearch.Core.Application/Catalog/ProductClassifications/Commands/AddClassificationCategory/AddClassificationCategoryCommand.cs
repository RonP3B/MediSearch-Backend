using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.AddClassificationCategory;

[Authorize(Permission = PermissionCodes.CreateClassificationCategory)]
public sealed record AddClassificationCategoryCommand(Guid ClassificationId, string CategoryName)
    : ICommand<ClassificationCategoryDto>;
