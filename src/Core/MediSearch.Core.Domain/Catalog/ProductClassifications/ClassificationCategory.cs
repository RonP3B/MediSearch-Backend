namespace MediSearch.Core.Domain.Catalog.ProductClassifications;

public sealed class ClassificationCategory : BaseAuditableEntity
{
    private ClassificationCategory() { }

    public EntityId<ClassificationCategory> Id { get; private set; } =
        EntityId<ClassificationCategory>.Empty;
    public Name Name { get; private set; } = Name.Empty;
    public EntityId<ProductClassification> ClassificationId { get; private set; } =
        EntityId<ProductClassification>.Empty;

    public static ClassificationCategory Create(
        EntityId<ProductClassification> classificationId,
        Name name
    )
    {
        return new ClassificationCategory
        {
            Id = EntityId<ClassificationCategory>.New(),
            ClassificationId = EntityId<ProductClassification>.From(classificationId.Value),
            Name = name,
        };
    }

    public void UpdateName(Name newName)
    {
        Name = newName;
    }
}
