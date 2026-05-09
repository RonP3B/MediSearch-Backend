using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.RemoveClassificationCategory;

public sealed class RemoveClassificationCategoryCommandHandler(
    IProductClassificationRepository productClassificationRepository
) : ICommandHandler<RemoveClassificationCategoryCommand>
{
    private readonly IProductClassificationRepository _productClassificationRepository =
        productClassificationRepository;

    public async Task Handle(
        RemoveClassificationCategoryCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var productClassification = await _productClassificationRepository.GetByIdOrDefaultAsync(
            EntityId<ProductClassification>.From(cmd.ClassificationId),
            cancellationToken
        );

        if (productClassification is null)
        {
            throw NotFoundException.Entity(
                nameof(ProductClassification),
                nameof(ProductClassification.Id),
                cmd.ClassificationId
            );
        }

        productClassification.RemoveCategory(EntityId<ClassificationCategory>.From(cmd.CategoryId));
    }
}
