using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;
using MediSearch.Core.Domain.Catalog.Products.ValueObjects;
using MediSearch.Core.Domain.Companies;

namespace MediSearch.Core.Domain.Catalog.Products;

public sealed class Product : BaseAuditableEntity
{
    private readonly List<AssetKey> _imageKeys = [];
    private readonly List<EntityId<ClassificationCategory>> _categoryIds = [];
    private const int MaxImageKeyCount = 7;

    private Product() { }

    public EntityId<Product> Id { get; private set; } = EntityId<Product>.Empty;
    public ProductName Name { get; private set; } = ProductName.Empty;
    public CleanText Description { get; private set; } = CleanText.Empty;
    public Price Price { get; private set; } = Price.Empty;
    public Quantity Quantity { get; private set; } = Quantity.Empty;
    public EntityId<Company> CompanyId { get; private set; } = EntityId<Company>.Empty;
    public EntityId<ProductClassification> ClassificationId { get; private set; } =
        EntityId<ProductClassification>.Empty;

    public IReadOnlyCollection<AssetKey> ImageKeys => _imageKeys.AsReadOnly();
    public IReadOnlyCollection<EntityId<ClassificationCategory>> CategoryIds =>
        _categoryIds.AsReadOnly();

    public static Product Create(
        ProductName name,
        CleanText description,
        Price price,
        Quantity quantity,
        EntityId<Company> companyId,
        EntityId<ProductClassification> classificationId,
        IReadOnlyList<AssetKey> imageKeys,
        IReadOnlyList<EntityId<ClassificationCategory>> categoryIds
    )
    {
        if (imageKeys is null || imageKeys.Count == 0)
        {
            throw new BusinessRuleException(nameof(Product), ProductErrorCodes.ImageKeyRequired);
        }

        if (imageKeys.Count > MaxImageKeyCount)
        {
            throw new BusinessRuleException(
                nameof(Product),
                new ErrorCode(
                    ProductErrorCodes.ImageKeyLimitExceeded,
                    new() { [nameof(MaxImageKeyCount)] = $"{MaxImageKeyCount}" }
                )
            );
        }

        if (categoryIds is null || categoryIds.Count == 0)
        {
            throw new BusinessRuleException(nameof(Product), ProductErrorCodes.CategoryIdRequired);
        }

        var product = new Product
        {
            Id = EntityId<Product>.New(),
            Name = name,
            Description = description,
            Price = price,
            Quantity = quantity,
            CompanyId = companyId,
            ClassificationId = classificationId,
        };

        product._imageKeys.AddRange(imageKeys);
        product._categoryIds.AddRange(categoryIds);

        product.AddDomainEvent(
            new ProductCreatedDomainEvent(
                product.Id,
                product.CompanyId,
                product.Name,
                product.Price.ToString()
            )
        );

        return product;
    }

    public void UpdateDetails(
        ProductName name,
        CleanText description,
        Price price,
        Quantity quantity,
        EntityId<ProductClassification> classificationId,
        IReadOnlyList<EntityId<ClassificationCategory>> categoryIds
    )
    {
        Name = name;
        Description = description;
        UpdatePrice(price);
        UpdateQuantity(quantity);
        UpdateClassification(classificationId, categoryIds);
    }

    public void UpdateImageKeys(IReadOnlyList<AssetKey> newImageKeys)
    {
        if (newImageKeys.Count == 0)
        {
            throw new BusinessRuleException(nameof(ImageKeys), ProductErrorCodes.ImageKeyRequired);
        }

        if (newImageKeys.Count > MaxImageKeyCount)
        {
            throw new BusinessRuleException(
                nameof(Product),
                new ErrorCode(
                    ProductErrorCodes.ImageKeyLimitExceeded,
                    new() { [nameof(MaxImageKeyCount)] = $"{MaxImageKeyCount}" }
                )
            );
        }

        if (_imageKeys.SequenceEqual(newImageKeys))
        {
            return;
        }

        List<AssetKey> removedKeys = [.. _imageKeys.Except(newImageKeys)];

        _imageKeys.Clear();
        _imageKeys.AddRange(newImageKeys);

        if (removedKeys.Count > 0)
        {
            AddDomainEvent(
                new ProductImagesRemovedDomainEvent(Id, [.. removedKeys.Select(x => x.Key)])
            );
        }
    }

    public void Delete()
    {
        AddDomainEvent(
            new ProductDeletedDomainEvent(Id, CompanyId, Name, [.. _imageKeys.Select(x => x.Key)])
        );
    }

    private void UpdatePrice(Price newPrice)
    {
        if (newPrice == Price)
        {
            return;
        }

        var previous = Price;
        Price = newPrice;
        AddDomainEvent(
            new ProductPriceChangedDomainEvent(
                Id,
                CompanyId,
                Name,
                previous.ToString(),
                newPrice.ToString()
            )
        );
    }

    private void UpdateQuantity(Quantity newQuantity)
    {
        bool wasOutOfStock = Quantity.Value == 0;
        bool isNowOutOfStock = newQuantity.Value == 0;

        Quantity = newQuantity;

        if (wasOutOfStock && !isNowOutOfStock)
        {
            AddDomainEvent(new ProductBackInStockDomainEvent(Id, CompanyId, Name));
        }
        else if (!wasOutOfStock && isNowOutOfStock)
        {
            AddDomainEvent(new ProductOutOfStockDomainEvent(Id, CompanyId, Name));
        }
    }

    private void UpdateClassification(
        EntityId<ProductClassification> newClassificationId,
        IReadOnlyList<EntityId<ClassificationCategory>> categoryIds
    )
    {
        if (categoryIds is null || categoryIds.Count == 0)
        {
            throw new BusinessRuleException(
                nameof(CategoryIds),
                ProductErrorCodes.CategoryIdRequired
            );
        }

        if (
            newClassificationId == ClassificationId
            && _categoryIds.ToHashSet().SetEquals(categoryIds)
        )
        {
            return;
        }

        ClassificationId = newClassificationId;
        _categoryIds.Clear();
        _categoryIds.AddRange(categoryIds);
    }
}
