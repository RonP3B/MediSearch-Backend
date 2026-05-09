using MediSearch.Core.Application.Shared.Ports;
using Microsoft.Extensions.Options;

namespace MediSearch.Infrastructure.Base.FileStorage.MediSearchApi;

internal sealed class MediSearchApiFileStorage(
    IOptions<MediSearchApiFileStorageOptions> apiFileStorageOptions
) : IFileStorageService
{
    private readonly string _storagePath = apiFileStorageOptions.Value.StoragePath;

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        string imageIdentifier = fileName.ToFileKey();

        string filePath = Path.Combine(_storagePath, imageIdentifier);

        using (FileStream fileStreamToWrite = new(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(fileStreamToWrite, cancellationToken);
        }

        return imageIdentifier;
    }

    public async Task<string[]> SaveFilesAsync(
        IEnumerable<(Stream FileStream, string FileName)> files,
        CancellationToken cancellationToken = default
    )
    {
        List<string> savedFiles = [];

        IEnumerable<Task<string>> tasks = files.Select(async file =>
        {
            string imageIdentifier = file.FileName.ToFileKey();

            string filePath = Path.Combine(_storagePath, imageIdentifier);

            using (FileStream fileStreamToWrite = new(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.FileStream.CopyToAsync(fileStreamToWrite, cancellationToken);
            }

            return imageIdentifier;
        });

        savedFiles.AddRange(await Task.WhenAll(tasks));

        return [.. savedFiles];
    }

    public Task DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(_storagePath, fileKey);

        if (!File.Exists(filePath))
        {
            return Task.CompletedTask;
        }

        File.Delete(filePath);

        return Task.CompletedTask;
    }

    public Task DeleteFilesAsync(
        IEnumerable<string> fileKeys,
        CancellationToken cancellationToken = default
    )
    {
        foreach (string fileKey in fileKeys)
        {
            DeleteFileAsync(fileKey, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
