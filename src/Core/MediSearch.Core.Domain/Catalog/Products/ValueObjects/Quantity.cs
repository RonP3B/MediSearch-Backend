namespace MediSearch.Core.Domain.Catalog.Products.ValueObjects;

public sealed class Quantity : ValueObject
{
    private Quantity(int value)
    {
        Value = value;
    }

    public static Result<Quantity> TryFrom(int value)
    {
        return value < 0
            ? Result<Quantity>.Fail([new(nameof(Quantity), ProductErrorCodes.QuantityBelowZero)])
            : Result<Quantity>.Ok(new Quantity(value));
    }

    public static Quantity From(int value)
    {
        var result = TryFrom(value);

        if (!result.IsSuccess)
        {
            throw new InvalidValueObjectStateException<Quantity>();
        }

        return result.Value;
    }

    public int Value { get; }

    public static implicit operator int(Quantity qty) => qty.Value;

    public override string ToString() => Value.ToString();

    internal static Quantity Empty { get; } = new Quantity(-1);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
