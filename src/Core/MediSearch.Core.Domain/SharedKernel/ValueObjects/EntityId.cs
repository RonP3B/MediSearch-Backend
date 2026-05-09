namespace MediSearch.Core.Domain.SharedKernel.ValueObjects;

public class EntityId<TEntity> : ValueObject
    where TEntity : BaseEntity
{
    private EntityId(Guid value)
    {
        Value = value;
    }

    public static Result<EntityId<TEntity>> TryFrom(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Result<EntityId<TEntity>>.Fail(
                [new($"{typeof(TEntity).Name}Id", DomainErrorCodes.EmptyId)]
            );
        }

        return Result<EntityId<TEntity>>.Ok(new EntityId<TEntity>(value));
    }

    public static EntityId<TEntity> From(Guid value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<EntityId<TEntity>>();
        }

        return result.Value;
    }

    public Guid Value { get; private set; }

    public static EntityId<TEntity> New() => new(Guid.CreateVersion7());

    public static implicit operator Guid(EntityId<TEntity> id) => id.Value;

    public static implicit operator string(EntityId<TEntity> id) => id.Value.ToString();

    public override string ToString() => Value.ToString();

    internal static EntityId<TEntity> Empty { get; } = new EntityId<TEntity>(Guid.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
