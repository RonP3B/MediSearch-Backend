using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Domain.Catalog.ProductClassifications;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Commands.CreateProductClassification;

public sealed class CreateProductClassificationCommandHandler(
    IProductClassificationRepository productClassificationRepository
) : ICommandHandler<CreateProductClassificationCommand, ProductClassificationDto>
{
    private readonly IProductClassificationRepository _productClassificationRepository =
        productClassificationRepository;

    public Task<ProductClassificationDto> Handle(
        CreateProductClassificationCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var productClassification = ProductClassification.Create(Name.From(cmd.Name));

        _productClassificationRepository.Add(productClassification);

        return Task.FromResult(productClassification.Adapt<ProductClassificationDto>());
    }
}
