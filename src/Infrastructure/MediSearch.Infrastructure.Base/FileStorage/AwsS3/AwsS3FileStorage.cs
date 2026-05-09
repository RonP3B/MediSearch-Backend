using Amazon.S3;
using Amazon.S3.Model;
using MediSearch.Core.Application.Shared.Ports;
using Microsoft.Extensions.Options;

namespace MediSearch.Infrastructure.Base.FileStorage.AwsS3;

internal sealed class AwsS3FileStorage(IAmazonS3 s3Client, IOptions<AwsS3Options> options)
    : IFileStorageService
{
    private readonly string _bucketName = options.Value.BucketName;
    private readonly IAmazonS3 _s3Client = s3Client;

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        string key = fileName.ToFileKey();

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            AutoCloseStream = false,
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return key;
    }

    public async Task<string[]> SaveFilesAsync(
        IEnumerable<(Stream FileStream, string FileName)> files,
        CancellationToken cancellationToken = default
    )
    {
        var tasks = files.Select(f => SaveFileAsync(f.FileStream, f.FileName, cancellationToken));
        return await Task.WhenAll(tasks);
    }

    public async Task DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest { BucketName = _bucketName, Key = fileKey };
        await _s3Client.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task DeleteFilesAsync(
        IEnumerable<string> fileKeys,
        CancellationToken cancellationToken = default
    )
    {
        var objects = fileKeys.Select(key => new KeyVersion { Key = key }).ToList();
        var request = new DeleteObjectsRequest { BucketName = _bucketName, Objects = objects };
        await _s3Client.DeleteObjectsAsync(request, cancellationToken);
    }
}
