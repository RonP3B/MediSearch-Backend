using System.Text.Json.Serialization;

namespace MediSearch.Infrastructure.Base.FileStorage.UploadThing;

internal sealed class PresignedUrlResponse
{
    [JsonPropertyName("fileKey")]
    public string FileKey { get; init; } = string.Empty;

    [JsonPropertyName("uploadUrl")]
    public string UploadUrl { get; init; } = string.Empty;
}
