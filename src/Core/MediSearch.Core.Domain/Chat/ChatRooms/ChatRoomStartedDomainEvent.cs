namespace MediSearch.Core.Domain.Chat.ChatRooms;

public sealed class ChatRoomStartedDomainEvent(
    Guid chatRoomId,
    Guid recipientAgentId,
    int recipientAgentTypeId
) : DomainEvent
{
    public Guid ChatRoomId { get; } = chatRoomId;
    public Guid RecipientAgentId { get; } = recipientAgentId;
    public int RecipientAgentTypeId { get; } = recipientAgentTypeId;
}
