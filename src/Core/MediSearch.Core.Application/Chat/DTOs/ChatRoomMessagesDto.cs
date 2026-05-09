namespace MediSearch.Core.Application.Chat.DTOs;

public sealed record ChatRoomMessagesDto
{
    public required Guid Id { get; init; }
    public required AgentSummaryDto Recipient { get; init; }
    public required IReadOnlyList<MessageDto> Messages { get; init; }
}
