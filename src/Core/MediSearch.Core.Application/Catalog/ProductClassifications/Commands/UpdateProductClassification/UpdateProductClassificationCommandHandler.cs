using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateProductClassification;

public sealed class UpdateProductClassificationCommandHandler(
    IProductClassificationRepository productClassificationRepository
) : ICommandHandler<UpdateProductClassificationCommand, ProductClassificationDto>
{
    private readonly IProductClassificationRepository _productClassificationRepository =
        productClassificationRepository;

    public async Task<ProductClassificationDto> Handle(
        UpdateProductClassificationCommand cmd,
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

        productClassification.UpdateName(Name.From(cmd.Name));

        return productClassification.Adapt<ProductClassificationDto>();
    }
}
