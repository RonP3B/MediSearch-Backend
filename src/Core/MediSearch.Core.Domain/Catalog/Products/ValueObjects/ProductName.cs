using System.Text.RegularExpressions;

namespace MediSearch.Core.Domain.Catalog.Products.ValueObjects;

public sealed partial class ProductName : ValueObject
{
    private const int MinLength = 2;
    private const int MaxLength = 100;

    private ProductName(string value)
    {
        Value = value;
        Normalized = value.ToLowerInvariant();
    }

    public static Result<ProductName> TryFrom(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<ProductName>.Fail(
                [new(nameof(ProductName), DomainErrorCodes.EmptyField)]
            );
        }

        var failures = new List<DomainFailure>();

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            failures.Add(
                new(
                    nameof(ProductName),
                    new ErrorCode(
                        ProductErrorCodes.ProductNameTooLong,
                        new() { [nameof(MaxLength)] = $"{MaxLength}" }
                    )
                )
            );
        }

        if (value.Length < MinLength)
        {
            failures.Add(
                new(
                    nameof(ProductName),
                    new ErrorCode(
                        ProductErrorCodes.ProductNameTooShort,
                        new() { [nameof(MinLength)] = $"{MinLength}" }
                    )
                )
            );
        }

        if (!ValidProductNameRegex().IsMatch(value))
        {
            failures.Add(new(nameof(ProductName), ProductErrorCodes.ProductNameInvalid));
        }

        return failures.Count > 0
            ? Result<ProductName>.Fail(failures)
            : Result<ProductName>.Ok(new ProductName(value));
    }

    public static ProductName From(string value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<ProductName>();
        }

        return result.Value;
    }

    public string Value { get; private set; }

    /// <summary>
    /// Lower-cased form of the value. It is what uniqueness checks, lookups and equality
    /// use, so two values that differ only in casing are the same value, while
    /// <see cref="Value"/> still holds exactly what the user typed.
    /// </summary>
    public string Normalized { get; private set; }

    public static implicit operator string(ProductName productName) => productName.Value;

    public override string ToString() => Value;

    internal static ProductName Empty { get; } = new ProductName(string.Empty);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Normalized;
    }

    [GeneratedRegex(@"^[A-Za-zÁÉÍÓÚÑáéíóúñ0-9\s\-().\/+]+$", RegexOptions.Compiled)]
    private static partial Regex ValidProductNameRegex();
}
