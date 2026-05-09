namespace MediSearch.Core.Domain.SharedKernel.Bases;

public sealed class ErrorCode
{
    public string Key { get; }
    public IReadOnlyDictionary<string, string> Parameters { get; }

    public ErrorCode(string key, Dictionary<string, string>? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(key);

        key = key.Trim();

        if (key.Length == 0)
        {
            throw new ArgumentException("Error code key cannot be empty.", nameof(key));
        }

        Parameters = parameters is null ? [] : new Dictionary<string, string>(parameters);

        foreach (var kv in Parameters)
        {
            if (kv.Key is null || kv.Value is null)
            {
                throw new ArgumentException(
                    "Error code parameters cannot contain null keys or values."
                );
            }
        }

        Key = key;
    }

    public static implicit operator ErrorCode(string key) => new(key);
}
