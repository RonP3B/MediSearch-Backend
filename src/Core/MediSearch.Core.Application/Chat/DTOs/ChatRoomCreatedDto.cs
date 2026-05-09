namespace MediSearch.Core.Application.Chat.DTOs;

public sealed record ChatRoomCreatedDto
{
    public required Guid Id { get; init; }
    public required AgentSummaryDto Recipient { get; init; }
}
