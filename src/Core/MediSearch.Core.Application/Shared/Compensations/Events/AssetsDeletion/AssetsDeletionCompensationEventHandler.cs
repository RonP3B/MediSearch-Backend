namespace MediSearch.Core.Application.Shared.Compensations.Events.AssetsDeletion;

public sealed class AssetsDeletionCompensationEventHandler(IFileStorageService fileStorageService)
    : CompensationEventHandler<AssetsDeletionCompensationEvent>
{
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public override Task Handle(
        AssetsDeletionCompensationEvent compensationEvent,
        CancellationToken cancellationToken
    ) => _fileStorageService.DeleteFilesAsync(compensationEvent.AssetKeys, cancellationToken);
}
