namespace MediSearch.Infrastructure.Communication.Email.Smtp;

internal sealed record SmtpOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
}
