using MediSearch.Core.Domain.Chat.Messages;

namespace MediSearch.Core.Domain.Chat.ChatRooms;

public sealed class ChatRoom : BaseAuditableEntity
{
    private readonly List<ChatRoomParticipant> _participants = [];

    private ChatRoom() { }

    public EntityId<ChatRoom> Id { get; private set; } = EntityId<ChatRoom>.Empty;
    public EntityId<Message>? LastMessageId { get; private set; }
    public IReadOnlyCollection<ChatRoomParticipant> Participants => _participants.AsReadOnly();

    public static ChatRoom Create(Agent creatorAgent, Agent recipientAgent)
    {
        if (creatorAgent == recipientAgent)
        {
            throw new BusinessRuleException(
                nameof(Participants),
                ChatRoomErrorCodes.ParticipantsMustBeDistinct
            );
        }

        var chatRoom = new ChatRoom { Id = EntityId<ChatRoom>.New() };

        chatRoom._participants.AddRange(
            [new ChatRoomParticipant(creatorAgent), new ChatRoomParticipant(recipientAgent)]
        );

        chatRoom.AddDomainEvent(
            new ChatRoomStartedDomainEvent(
                chatRoom.Id,
                recipientAgent.AgentId,
                recipientAgent.AgentTypeId
            )
        );

        return chatRoom;
    }

    public void UpdateLastMessage(EntityId<Message> lastMessageId)
    {
        LastMessageId = lastMessageId;
    }

    public void MarkAsRead(Agent agent, DateTimeOffset now)
    {
        var participant =
            _participants.FirstOrDefault(p => p.Agent == agent)
            ?? throw new BusinessRuleException(
                nameof(agent),
                ChatRoomErrorCodes.ParticipantNotFound
            );

        participant.UpdateLastChecked(now);
    }

    public bool HasParticipant(Agent agent) => _participants.Any(p => p.Agent == agent);
}
