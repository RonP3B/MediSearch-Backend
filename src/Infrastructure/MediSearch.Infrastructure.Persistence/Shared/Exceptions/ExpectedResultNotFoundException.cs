namespace MediSearch.Infrastructure.Persistence.Shared.Exceptions;

public sealed class ExpectedResultNotFoundException : Exception
{
    public ExpectedResultNotFoundException(string message, Exception? inner = null)
        : base(message, inner) { }
}
