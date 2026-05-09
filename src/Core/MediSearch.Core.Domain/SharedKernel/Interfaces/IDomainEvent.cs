namespace MediSearch.Core.Domain.SharedKernel.Interfaces;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}
