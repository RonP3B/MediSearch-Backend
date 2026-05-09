using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.Companies.ValueObjects;

public sealed partial class CompanyName : ValueObject
{
    private const int MinLength = 3;
    private const int MaxLength = 100;

    private CompanyName(string value)
    {
        Value = value;
    }

    public static Result<CompanyName> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<CompanyName>.Fail(
                [new(nameof(CompanyName), DomainErrorCodes.EmptyField)]
            );
        }

        var failures = new List<DomainFailure>();

        value = value.Trim().ToLowerInvariant();

        if (value.Length < MinLength)
        {
            failures.Add(
                new(
                    nameof(CompanyName),
                    new ErrorCode(
                        CompanyErrorCodes.CompanyNameTooShort,
                        new() { [nameof(MinLength)] = $"{MinLength}" }
                    )
                )
            );
        }

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(CompanyName),
                    new ErrorCode(
                        CompanyErrorCodes.CompanyNameTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (!ValidCompanyNameRegex().IsMatch(value))
        {
            failures.Add(new(nameof(CompanyName), CompanyErrorCodes.CompanyNameInvalid));
        }

        return failures.Count > 0
            ? Result<CompanyName>.Fail(failures)
            : Result<CompanyName>.Ok(new CompanyName(value));
    }

    public static CompanyName From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<CompanyName>();
        }

        return result.Value;
    }

    public string Value { get; private set; }

    public static implicit operator string(CompanyName name) => name.Value;

    public override string ToString() => Value;

    internal static CompanyName Empty { get; } = new CompanyName(string.Empty);

    [GeneratedRegex(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9\s&.'\-,]+$", RegexOptions.Compiled)]
    private static partial Regex ValidCompanyNameRegex();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
