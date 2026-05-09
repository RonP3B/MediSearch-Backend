using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.Companies.ValueObjects;

public sealed partial class CompanyCeoName : ValueObject
{
    private const int MinLength = 2;
    private const int MaxLength = 150;

    private CompanyCeoName(string value)
    {
        Value = value;
    }

    public static Result<CompanyCeoName> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<CompanyCeoName>.Fail(
                [new(nameof(CompanyCeoName), DomainErrorCodes.EmptyField)]
            );
        }

        var failures = new List<DomainFailure>();

        value = value.Trim().ToLowerInvariant();

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(CompanyCeoName),
                    new ErrorCode(
                        CompanyErrorCodes.CeoNameTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (value.Length < MinLength)
        {
            failures.Add(
                new(
                    nameof(CompanyCeoName),
                    new ErrorCode(
                        CompanyErrorCodes.CeoNameTooShort,
                        new() { [nameof(MinLength)] = $"{MinLength}" }
                    )
                )
            );
        }

        if (!ValidCeoNameRegex().IsMatch(value))
        {
            failures.Add(new(nameof(CompanyCeoName), CompanyErrorCodes.CeoNameInvalid));
        }

        return failures.Count > 0
            ? Result<CompanyCeoName>.Fail(failures)
            : Result<CompanyCeoName>.Ok(new CompanyCeoName(value));
    }

    public static CompanyCeoName From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<CompanyCeoName>();
        }

        return result.Value;
    }

    public string Value { get; private set; }

    public static implicit operator string(CompanyCeoName ceoName) => ceoName.Value;

    public override string ToString() => Value;

    internal static CompanyCeoName Empty { get; } = new CompanyCeoName(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[A-Za-zÁÉÍÓÚÑáéíóúñ\s]+$", RegexOptions.Compiled)]
    private static partial Regex ValidCeoNameRegex();
}
