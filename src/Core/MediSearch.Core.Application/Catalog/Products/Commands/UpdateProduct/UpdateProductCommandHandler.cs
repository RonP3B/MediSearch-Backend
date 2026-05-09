using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Catalog.Products.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IFileStorageService fileStorageService,
    IEventBus eventBus,
    ICompensationManager compensationManager,
    ICurrentUser currentUser
) : ICommandHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICompensationManager _compensationManager = compensationManager;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ProductDto> Handle(
        UpdateProductCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetByIdOrDefaultAsync(
            EntityId<Product>.From(cmd.Id),
            cancellationToken
        );

        if (product is null)
        {
            throw NotFoundException.Entity(nameof(Product), nameof(Product.Id), cmd.Id);
        }

        if (product.CompanyId.Value != _currentUser.GetAuthenticatedUserCompanyId())
        {
            throw new ForbiddenAccessException();
        }

        product.UpdateDetails(
            name: ProductName.From(cmd.Name),
            description: CleanText.From(cmd.Description),
            price: Price.From(cmd.PriceAmount, cmd.PriceCurrency),
            quantity: Quantity.From(cmd.Quantity),
            classificationId: EntityId<ProductClassification>.From(cmd.ClassificationId),
            categoryIds: [.. cmd.CategoryIds.Select(EntityId<ClassificationCategory>.From)]
        );

        if (cmd.ImageFiles.Count > 0)
        {
            string[] newImageKeys = await _compensationManager.ExecuteAsync(
                new SaveAssetsCompensableOperation(
                    fileDtos: cmd.ImageFiles,
                    fileStorageService: _fileStorageService,
                    eventBus: _eventBus
                ),
                cancellationToken
            );

            product.UpdateImageKeys([.. newImageKeys.Select(AssetKey.From)]);
        }

        return product.Adapt<ProductDto>();
    }
}
