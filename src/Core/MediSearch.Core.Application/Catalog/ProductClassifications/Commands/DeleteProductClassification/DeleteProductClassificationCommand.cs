namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.DeleteProductClassification;

[Authorize(Permission = PermissionCodes.DeleteProductClassification)]
public sealed record DeleteProductClassificationCommand(Guid Id) : ICommand;
