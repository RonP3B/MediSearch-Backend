namespace MediSearch.Infrastructure.Communication.Email.APIs;

internal sealed record EmailServiceOptions
{
    public required string ApiKey { get; init; }
    public required string FromName { get; init; }
    public required string FromEmail { get; init; }
}
