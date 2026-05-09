namespace MediSearch.Core.Application.Shared.Compensations.Events.AssetDeletion;

public sealed record AssetDeletionCompensationEvent(string AssetKey) : CompensationEvent;
