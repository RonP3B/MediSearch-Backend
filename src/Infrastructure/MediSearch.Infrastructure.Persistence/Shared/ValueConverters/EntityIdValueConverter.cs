using MediSearch.Core.Domain.SharedKernel.Bases;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Shared.ValueConverters;

internal sealed class EntityIdValueConverter<TEntity> : ValueConverter<EntityId<TEntity>, Guid>
    where TEntity : BaseEntity
{
    public EntityIdValueConverter()
        : base(entityId => entityId.Value, value => EntityId<TEntity>.From(value)) { }
}
