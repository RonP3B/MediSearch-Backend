namespace MediSearch.Core.Application.Catalog.Products.Commands.DeleteProduct;

[Authorize(Permission = PermissionCodes.RemoveProduct)]
public sealed record DeleteProductCommand(Guid ProductId) : ICommand;
