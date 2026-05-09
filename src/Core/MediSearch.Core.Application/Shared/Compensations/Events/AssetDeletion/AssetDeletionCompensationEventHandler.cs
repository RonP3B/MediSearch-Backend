namespace MediSearch.Core.Application.Shared.Compensations.Events.AssetDeletion;

public sealed class AssetDeletionCompensationEventHandler(IFileStorageService fileStorageService)
    : CompensationEventHandler<AssetDeletionCompensationEvent>
{
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public override Task Handle(
        AssetDeletionCompensationEvent compensationEvent,
        CancellationToken cancellationToken
    ) => _fileStorageService.DeleteFileAsync(compensationEvent.AssetKey, cancellationToken);
}
