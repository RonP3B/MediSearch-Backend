using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ICurrentUser currentUser
) : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(DeleteProductCommand cmd, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdOrDefaultAsync(
            EntityId<Product>.From(cmd.ProductId),
            cancellationToken
        );

        if (product is null)
        {
            throw NotFoundException.Entity(nameof(Product), nameof(Product.Id), cmd.ProductId);
        }

        if (product.CompanyId.Value != _currentUser.GetAuthenticatedUserCompanyId())
        {
            throw new ForbiddenAccessException();
        }

        product.Delete();

        _productRepository.Remove(product);
    }
}
