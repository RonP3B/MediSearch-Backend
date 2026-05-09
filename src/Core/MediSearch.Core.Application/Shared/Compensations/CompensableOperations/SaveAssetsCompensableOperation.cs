using MediSearch.Core.Application.Shared.Compensations.Events.AssetsDeletion;

namespace MediSearch.Core.Application.Shared.Compensations.CompensableOperations;

internal sealed class SaveAssetsCompensableOperation(
    IReadOnlyList<FileDto> fileDtos,
    IFileStorageService fileStorageService,
    IEventBus eventBus
) : ICompensableOperation<string[]>
{
    public async Task<string[]> ExecuteAsync(CancellationToken cancellationToken)
    {
        return await fileStorageService.SaveFilesAsync(
            fileDtos.Select(file => (file.Content, file.FileName)),
            cancellationToken
        );
    }

    public async Task CompensateAsync(string[] assetKeys)
    {
        await eventBus.PublishAsync(new AssetsDeletionCompensationEvent(assetKeys));
    }
}
