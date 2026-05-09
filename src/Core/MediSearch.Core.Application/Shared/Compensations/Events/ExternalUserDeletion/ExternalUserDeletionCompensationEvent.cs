namespace MediSearch.Core.Application.Shared.Compensations.Events.ExternalUserDeletion;

public sealed record ExternalUserDeletionCompensationEvent(string ExternalUserId)
    : CompensationEvent;
