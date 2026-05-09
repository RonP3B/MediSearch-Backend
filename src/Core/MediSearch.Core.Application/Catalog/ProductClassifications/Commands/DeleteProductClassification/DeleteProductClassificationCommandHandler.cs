using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.DeleteProductClassification;

public sealed class DeleteProductClassificationCommandHandler(
    IProductClassificationRepository productClassificationRepository
) : ICommandHandler<DeleteProductClassificationCommand>
{
    private readonly IProductClassificationRepository _productClassificationRepository =
        productClassificationRepository;

    public async Task Handle(
        DeleteProductClassificationCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var productClassification = await _productClassificationRepository.GetByIdOrDefaultAsync(
            EntityId<ProductClassification>.From(cmd.Id),
            cancellationToken
        );

        if (productClassification is null)
        {
            throw NotFoundException.Entity(
                nameof(ProductClassification),
                nameof(ProductClassification.Id),
                cmd.Id
            );
        }

        productClassification.Delete();
        _productClassificationRepository.Remove(productClassification);
    }
}
