using MediSearch.Core.Domain.SharedKernel.Bases;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Shared.ValueConverters;

internal sealed class NullableEntityIdValueConverter<TEntity>
    : NullableSingleFieldValueObjectConverter<EntityId<TEntity>, Guid>
    where TEntity : BaseEntity
{
    public NullableEntityIdValueConverter()
        : base(entityId => entityId.Value, EntityId<TEntity>.From) { }
}
