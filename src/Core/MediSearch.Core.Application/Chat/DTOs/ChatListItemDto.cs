namespace MediSearch.Core.Application.Chat.DTOs;

public sealed record ChatListItemDto
{
    public required Guid Id { get; init; }
    public required AgentSummaryDto Recipient { get; init; }
    public string? LastMessageText { get; init; } = null;
    public string? LastMessageMediaAssetKey { get; init; } = null;
    public string? LastMessageMediaType { get; init; } = null;
    public DateTimeOffset? LastMessageSentDate { get; init; } = null;
    public required int UnreadCount { get; init; } = 0;
}
