using MediSearch.Core.Domain.Chat.ChatRooms;

namespace MediSearch.Core.Domain.Chat.Messages;

public sealed class Message : BaseAuditableEntity
{
    private Message() { }

    public EntityId<Message> Id { get; private set; } = EntityId<Message>.Empty;
    public EntityId<ChatRoom> ChatRoomId { get; private set; } = EntityId<ChatRoom>.Empty;
    public DateTimeOffset MessageSentDate { get; private set; }
    public Agent SenderAgent { get; private set; } = Agent.Empty;
    public CleanText? TextContent { get; private set; }
    public MessageMediaContent? MediaContent { get; private set; }

    public static Message Create(
        CleanText? textContent,
        Agent senderAgent,
        EntityId<ChatRoom> chatRoomId,
        DateTimeOffset sentAt,
        MessageMediaContent? mediaContent = null
    )
    {
        var hasText = textContent is not null && textContent != CleanText.Empty;
        var hasMedia = mediaContent is not null;

        if (!hasText && !hasMedia)
        {
            throw new BusinessRuleException(
                nameof(Message),
                MessageErrorCodes.MessageMustHaveContent
            );
        }

        var message = new Message
        {
            Id = EntityId<Message>.New(),
            TextContent = textContent ?? CleanText.Empty,
            MediaContent = mediaContent,
            MessageSentDate = sentAt,
            SenderAgent = senderAgent,
            ChatRoomId = chatRoomId,
        };

        message.AddDomainEvent(
            new MessageSentDomainEvent(
                message.Id,
                message.ChatRoomId,
                message.SenderAgent.AgentId,
                message.MessageSentDate,
                message.TextContent?.Value,
                message.MediaContent?.ToString(),
                message.MediaContent?.MediaType.ToString()
            )
        );

        return message;
    }
}
