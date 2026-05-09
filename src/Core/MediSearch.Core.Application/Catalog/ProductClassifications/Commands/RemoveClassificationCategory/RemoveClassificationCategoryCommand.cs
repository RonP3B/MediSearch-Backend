namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.RemoveClassificationCategory;

[Authorize(Permission = PermissionCodes.DeleteClassificationCategory)]
public sealed record RemoveClassificationCategoryCommand(Guid ClassificationId, Guid CategoryId)
    : ICommand;
