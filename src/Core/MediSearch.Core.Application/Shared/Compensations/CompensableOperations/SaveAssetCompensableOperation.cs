using MediSearch.Core.Application.Shared.Compensations.Events.AssetDeletion;

namespace MediSearch.Core.Application.Shared.Compensations.CompensableOperations;

internal sealed class SaveAssetCompensableOperation(
    FileDto fileDto,
    IFileStorageService fileStorageService,
    IEventBus eventBus
) : ICompensableOperation<string>
{
    public async Task<string> ExecuteAsync(CancellationToken cancellationToken)
    {
        return await fileStorageService.SaveFileAsync(
            fileDto.Content,
            fileDto.FileName,
            cancellationToken
        );
    }

    public async Task CompensateAsync(string assetKey)
    {
        await eventBus.PublishAsync(new AssetDeletionCompensationEvent(assetKey));
    }
}
