namespace MediSearch.Core.Application.Chat.DTOs;

public sealed record MessageDto
{
    public required Guid Id { get; init; }
    public required Guid ChatRoomId { get; init; }
    public required DateTimeOffset MessageSentDate { get; init; }
    public required AgentSummaryDto Sender { get; init; }
    public string? TextContent { get; init; } = null;
    public string? MediaContentAssetKey { get; init; } = null;
    public string? MediaContentType { get; init; } = null;
}
