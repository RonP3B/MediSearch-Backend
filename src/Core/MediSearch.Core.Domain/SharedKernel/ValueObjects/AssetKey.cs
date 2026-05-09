using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.SharedKernel.ValueObjects;

public sealed partial class AssetKey : ValueObject
{
    private const int MaxLength = 500;

    private AssetKey(string key)
    {
        Key = key;
    }

    public static Result<AssetKey> TryFrom(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<AssetKey>.Fail([new(nameof(AssetKey), DomainErrorCodes.EmptyField)]);
        }

        var failures = new List<DomainFailure>();

        key = key.Trim();

        if (key.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(AssetKey),
                    new ErrorCode(
                        DomainErrorCodes.AssetKeyTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (!ValidAssetKeyRegex().IsMatch(key))
        {
            failures.Add(new(nameof(AssetKey), DomainErrorCodes.InvalidAssetKeyFormat));
        }

        return failures.Count > 0
            ? Result<AssetKey>.Fail(failures)
            : Result<AssetKey>.Ok(new AssetKey(key));
    }

    public static AssetKey From(string key)
    {
        var result = TryFrom(key);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<AssetKey>();
        }

        return result.Value;
    }

    public string Key { get; private set; }

    public static implicit operator string(AssetKey key) => key.ToString();

    public override string ToString() => Key;

    internal static AssetKey Empty { get; } = new AssetKey(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;
    }

    [GeneratedRegex(
        @"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}_
        [\w.\-]+\.(jpg|jpeg|png|mp4|webm|mov|mkv|mp3|m4a|wav|flac|aac|ogg|opus)$",
        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace
    )]
    private static partial Regex ValidAssetKeyRegex();
}
