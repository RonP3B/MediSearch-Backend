namespace MediSearch.Core.Application.Shared.Ports;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    );

    Task<string[]> SaveFilesAsync(
        IEnumerable<(Stream FileStream, string FileName)> files,
        CancellationToken cancellationToken = default
    );

    Task DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default);

    Task DeleteFilesAsync(
        IEnumerable<string> fileKeys,
        CancellationToken cancellationToken = default
    );
}
