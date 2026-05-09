namespace MediSearch.Core.Domain.SharedKernel.Exceptions;

internal sealed class InvalidValueObjectStateException<TValueObject>()
    : DomainException($"Invalid state for value object '{typeof(TValueObject).Name}'")
    where TValueObject : ValueObject { }
