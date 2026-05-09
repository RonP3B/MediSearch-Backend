using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Catalog.Products.ValueObjects;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IFileStorageService fileStorageService,
    IEventBus eventBus,
    ICompensationManager compensationManager,
    ICurrentUser currentUser
) : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICompensationManager _compensationManager = compensationManager;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ProductDto> Handle(
        CreateProductCommand cmd,
        CancellationToken cancellationToken
    )
    {
        string[] imageKeys = await _compensationManager.ExecuteAsync(
            new SaveAssetsCompensableOperation(
                fileDtos: cmd.ImageFiles,
                fileStorageService: _fileStorageService,
                eventBus: _eventBus
            ),
            cancellationToken
        );

        Product product = Product.Create(
            companyId: EntityId<Company>.From(_currentUser.GetAuthenticatedUserCompanyId()),
            classificationId: EntityId<ProductClassification>.From(cmd.ClassificationId),
            name: ProductName.From(cmd.Name),
            description: CleanText.From(cmd.Description),
            price: Price.From(cmd.PriceAmount, cmd.PriceCurrency),
            quantity: Quantity.From(cmd.Quantity),
            imageKeys: [.. imageKeys.Select(AssetKey.From)],
            categoryIds: [.. cmd.CategoryIds.Select(EntityId<ClassificationCategory>.From)]
        );

        _productRepository.Add(product);

        return product.Adapt<ProductDto>();
    }
}
