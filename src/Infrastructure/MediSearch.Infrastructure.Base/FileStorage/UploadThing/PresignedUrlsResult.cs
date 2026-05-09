using System.Text.Json.Serialization;

namespace MediSearch.Infrastructure.Base.FileStorage.UploadThing;

internal sealed class PresignedUrlsResult
{
    [JsonPropertyName("data")]
    public List<PresignedUrlResponse> Data { get; init; } = [];
}
