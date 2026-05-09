namespace MediSearch.Core.Domain.Chat.Messages;

public sealed class MessageMediaContent : ValueObject
{
    private MessageMediaContent(AssetKey assetKey, MessageMediaType mediaType)
    {
        AssetKey = assetKey;
        MediaType = mediaType;
    }

    public static Result<MessageMediaContent> TryFrom(AssetKey assetKey)
    {
        if (assetKey is null)
        {
            return Result<MessageMediaContent>.Fail(
                new DomainFailure(nameof(AssetKey), DomainErrorCodes.EmptyField)
            );
        }

        string extension = ExtractExtension(assetKey);

        var mediaType = string.IsNullOrEmpty(extension) ? null : InferMediaType(extension);

        if (!mediaType.HasValue)
        {
            return Result<MessageMediaContent>.Fail(
                new DomainFailure(
                    nameof(MessageMediaContent),
                    MessageErrorCodes.MediaTypeDoesNotMatchAsset
                )
            );
        }

        return Result<MessageMediaContent>.Ok(new MessageMediaContent(assetKey, mediaType.Value));
    }

    public static MessageMediaContent From(AssetKey assetKey)
    {
        var result = TryFrom(assetKey);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<MessageMediaContent>();
        }

        return result.Value;
    }

    public AssetKey AssetKey { get; private set; }
    public MessageMediaType MediaType { get; private set; }

    private static MessageMediaType? InferMediaType(string ext)
    {
        foreach (var kv in MediaExtensions)
        {
            if (kv.Value.Contains(ext))
                return kv.Key;
        }

        return null;
    }

    private static string ExtractExtension(AssetKey assetKey)
    {
        int lastDot = assetKey.Key.LastIndexOf('.');

        if (lastDot < 0 || lastDot == assetKey.Key.Length - 1)
        {
            return string.Empty;
        }

        return assetKey.Key[lastDot..].ToLowerInvariant();
    }

    private static readonly Dictionary<MessageMediaType, HashSet<string>> MediaExtensions = new()
    {
        [MessageMediaType.Image] = new HashSet<string>(
            [".jpg", ".jpeg", ".png", ".webp", ".svg"],
            StringComparer.OrdinalIgnoreCase
        ),
        [MessageMediaType.Video] = new HashSet<string>(
            [".mp4", ".webm", ".mov", ".mkv"],
            StringComparer.OrdinalIgnoreCase
        ),
        [MessageMediaType.Audio] = new HashSet<string>(
            [".mp3", ".m4a", ".wav", ".flac", ".aac", ".ogg", ".opus"],
            StringComparer.OrdinalIgnoreCase
        ),
    };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AssetKey;
        yield return MediaType;
    }
}
