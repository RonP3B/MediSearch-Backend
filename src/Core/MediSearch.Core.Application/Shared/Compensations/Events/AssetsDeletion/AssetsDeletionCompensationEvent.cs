namespace MediSearch.Core.Application.Shared.Compensations.Events.AssetsDeletion;

public sealed record AssetsDeletionCompensationEvent(string[] AssetKeys) : CompensationEvent;
