namespace MediSearch.Infrastructure.Persistence.Shared.ValueConverters;

internal class NullableSingleFieldValueObjectConverter<TValueObject, TPrimitive>(
    Func<TValueObject, TPrimitive> to,
    Func<TPrimitive, TValueObject> from
)
    : ValueConverter<TValueObject?, TPrimitive?>(
        v => v != null ? to(v) : default,
        v => v != null ? from(v) : null
    )
    where TValueObject : class { }
