namespace MediSearch.Core.Domain.Companies.ValueObjects;

public sealed class Url : ValueObject
{
    private const int MaxLength = 500;

    private Url(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static Result<Url> TryFrom(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Url>.Fail([new(nameof(Url), DomainErrorCodes.EmptyField)]);
        }

        var failures = new List<DomainFailure>();

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(Url),
                    new ErrorCode(
                        DomainErrorCodes.UrlTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (!IsValidUrl(value))
        {
            failures.Add(new(nameof(Url), DomainErrorCodes.UrlInvalidFormat));
        }

        return failures.Count > 0 ? Result<Url>.Fail(failures) : Result<Url>.Ok(new Url(value));
    }

    public static Url From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Url>();
        }

        return result.Value;
    }

    public static implicit operator string(Url url) => url.Value;

    public override string ToString() => Value;

    internal static Url Empty { get; } = new Url(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
