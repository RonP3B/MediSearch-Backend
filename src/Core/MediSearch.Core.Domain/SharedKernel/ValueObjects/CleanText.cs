namespace MediSearch.Core.Domain.SharedKernel.ValueObjects;

public sealed class CleanText : ValueObject
{
    private const int MaxLength = 300;

    private CleanText(string value)
    {
        Value = value;
    }

    public static Result<CleanText> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<CleanText>.Fail([new(nameof(CleanText), DomainErrorCodes.EmptyField)]);
        }

        var failures = new List<DomainFailure>();

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(CleanText),
                    new ErrorCode(
                        DomainErrorCodes.CleanTextTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        bool containsForbiddenLanguage = ForbiddenWords.Any(word =>
            value.Contains(word, StringComparison.InvariantCultureIgnoreCase)
        );

        if (containsForbiddenLanguage)
        {
            failures.Add(new(nameof(CleanText), DomainErrorCodes.ContainsForbiddenLanguage));
        }

        return failures.Count > 0
            ? Result<CleanText>.Fail(failures)
            : Result<CleanText>.Ok(new CleanText(value));
    }

    public static CleanText From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<CleanText>();
        }

        return result.Value;
    }

    public string Value { get; private set; }

    public static implicit operator string(CleanText content) => content.Value;

    public override string ToString() => Value;

    internal static CleanText Empty { get; } = new CleanText(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private static readonly HashSet<string> ForbiddenWords =
    [
        "tonto",
        "tonta",
        "bobo",
        "boba",
        "zonzo",
        "zonza",
        "payaso",
        "payasa",
        "loco",
        "loca",
        "chiflado",
        "chiflada",
        "tarado",
        "tarada",
        "bruto",
        "bruta",
        "burro",
        "burra",
        "cochino",
        "cochina",
        "sucio",
        "sucia",
        "feo",
        "fea",
    ];
}
