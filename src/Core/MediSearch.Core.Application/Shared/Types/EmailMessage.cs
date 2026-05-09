namespace MediSearch.Core.Application.Shared.Types;

public sealed record EmailMessage(
    string ToEmailAddress,
    string Subject,
    string Body,
    string? ToDisplayName = null
);
