namespace MediSearch.Core.Domain.Chat.Messages;

public sealed class MessageSentDomainEvent(
    Guid messageId,
    Guid chatRoomId,
    Guid senderAgentId,
    DateTimeOffset sentDate,
    string? textContent,
    string? mediaContentAssetKey,
    string? mediaContentType
) : DomainEvent
{
    public Guid MessageId { get; } = messageId;
    public Guid ChatRoomId { get; } = chatRoomId;
    public Guid SenderAgentId { get; } = senderAgentId;
    public DateTimeOffset SentDate { get; } = sentDate;
    public string? TextContent { get; } = textContent;
    public string? MediaContentAssetKey { get; } = mediaContentAssetKey;
    public string? MediaContentType { get; } = mediaContentType;
}
