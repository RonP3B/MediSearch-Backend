using MediSearch.Core.Domain.Catalog.ProductClassifications.DomainEvents;

namespace MediSearch.Core.Domain.Catalog.ProductClassifications;

public sealed class ProductClassification : BaseAuditableEntity
{
    private readonly List<ClassificationCategory> _categories = [];

    private ProductClassification() { }

    public EntityId<ProductClassification> Id { get; private set; } =
        EntityId<ProductClassification>.Empty;

    public Name Name { get; private set; } = Name.Empty;
    public IReadOnlyCollection<ClassificationCategory> Categories => _categories.AsReadOnly();

    public static ProductClassification Create(Name name)
    {
        var productClassification = new ProductClassification
        {
            Id = EntityId<ProductClassification>.New(),
            Name = name,
        };

        productClassification.AddDomainEvent(
            new ProductClassificationCatalogChangedDomainEvent(productClassification.Id)
        );

        return productClassification;
    }

    public void UpdateName(Name name)
    {
        if (Name == name)
        {
            return;
        }

        Name = name;
        AddDomainEvent(new ProductClassificationCatalogChangedDomainEvent(Id));
    }

    public void AddCategory(ClassificationCategory category)
    {
        if (category.ClassificationId != Id)
        {
            throw new BusinessRuleException(
                nameof(Categories),
                ProductClassificationErrorCodes.CategoryNotInClassification
            );
        }

        if (_categories.Any(c => c.Id == category.Id))
        {
            throw new BusinessRuleException(
                nameof(Categories),
                ProductClassificationErrorCodes.CategoryAlreadyAdded
            );
        }

        if (
            _categories.Any(c =>
                string.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase)
            )
        )
        {
            throw new BusinessRuleException(
                nameof(Categories),
                ProductClassificationErrorCodes.CategoryNameAlreadyExists
            );
        }

        _categories.Add(category);
        AddDomainEvent(new ClassificationCategoriesChangedDomainEvent(Id));
    }

    public void RemoveCategory(EntityId<ClassificationCategory> categoryId)
    {
        var existing = _categories.FirstOrDefault(c => c.Id == categoryId);

        if (existing is null)
        {
            throw new BusinessRuleException(
                nameof(Categories),
                ProductClassificationErrorCodes.CategoryNotFound
            );
        }

        _categories.Remove(existing);
        AddDomainEvent(new ClassificationCategoriesChangedDomainEvent(Id));
    }

    public void UpdateCategoryName(EntityId<ClassificationCategory> categoryId, Name newName)
    {
        var category = _categories.FirstOrDefault(c => c.Id == categoryId);

        if (category is null)
        {
            throw new BusinessRuleException(
                nameof(Categories),
                ProductClassificationErrorCodes.CategoryNotFound
            );
        }

        if (
            _categories.Any(c =>
                c.Id != categoryId
                && string.Equals(c.Name, newName, StringComparison.OrdinalIgnoreCase)
            )
        )
        {
            throw new BusinessRuleException(
                nameof(Categories),
                ProductClassificationErrorCodes.CategoryNameAlreadyExists
            );
        }

        if (category.Name == newName)
        {
            return;
        }

        category.UpdateName(newName);
        AddDomainEvent(new ClassificationCategoriesChangedDomainEvent(Id));
    }

    public void Delete()
    {
        AddDomainEvent(new ProductClassificationDeletedDomainEvent(Id));
    }
}
