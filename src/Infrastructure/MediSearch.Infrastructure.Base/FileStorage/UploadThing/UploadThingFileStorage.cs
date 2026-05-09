using System.Net.Http.Headers;
using System.Text.Json;
using MediSearch.Core.Application.Shared.Ports;
using Microsoft.Extensions.Options;

namespace MediSearch.Infrastructure.Base.FileStorage.UploadThing;

internal sealed class UploadThingFileStorage(
    HttpClient httpClient,
    IOptions<UploadThingOptions> options
) : IFileStorageService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IOptions<UploadThingOptions> _options = options;
    private const string BaseUrl = "https://api.uploadthing.com";

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var results = await UploadFilesAsync([(fileStream, fileName)], cancellationToken);
        return results[0];
    }

    public async Task<string[]> SaveFilesAsync(
        IEnumerable<(Stream FileStream, string FileName)> files,
        CancellationToken cancellationToken = default
    )
    {
        return await UploadFilesAsync(files, cancellationToken);
    }

    public async Task DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        await DeleteFilesAsync([fileKey], cancellationToken);
    }

    public async Task DeleteFilesAsync(
        IEnumerable<string> fileKeys,
        CancellationToken cancellationToken = default
    )
    {
        var body = JsonSerializer.Serialize(new { fileKeys });

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/v6/files");

        request.Headers.Add("x-uploadthing-api-key", _options.Value.ApiKey);
        request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<string[]> UploadFilesAsync(
        IEnumerable<(Stream FileStream, string FileName)> files,
        CancellationToken cancellationToken
    )
    {
        var fileList = files.ToList();
        var presignedUrls = await GetPresignedUrlsAsync(fileList, cancellationToken);

        var uploadTasks = fileList
            .Zip(presignedUrls)
            .Select(pair =>
                UploadToPresignedUrlAsync(
                    pair.First.FileStream,
                    pair.Second.UploadUrl,
                    cancellationToken
                )
            );

        await Task.WhenAll(uploadTasks);

        return [.. presignedUrls.Select(p => p.FileKey)];
    }

    private async Task<List<PresignedUrlResponse>> GetPresignedUrlsAsync(
        List<(Stream FileStream, string FileName)> files,
        CancellationToken cancellationToken
    )
    {
        var body = JsonSerializer.Serialize(
            new
            {
                files = files.Select(f => new
                {
                    name = f.FileName.ToFileKey(),
                    size = f.FileStream.Length,
                }),
            }
        );

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/v6/uploadFiles");
        request.Headers.Add("x-uploadthing-api-key", _options.Value.ApiKey);
        request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<PresignedUrlsResult>(
            stream,
            cancellationToken: cancellationToken
        );

        return result?.Data
            ?? throw new InvalidOperationException("UploadThing returned no presigned URLs.");
    }

    private async Task UploadToPresignedUrlAsync(
        Stream fileStream,
        string presignedUrl,
        CancellationToken cancellationToken
    )
    {
        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        HttpResponseMessage response = await _httpClient.PutAsync(
            presignedUrl,
            content,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }
}
