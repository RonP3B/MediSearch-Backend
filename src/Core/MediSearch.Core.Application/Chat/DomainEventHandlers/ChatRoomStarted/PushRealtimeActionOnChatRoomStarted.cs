using MediSearch.Core.Application.Chat.Constants;
using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Core.Application.Chat.DomainEventHandlers.ChatRoomStarted;

public sealed class PushRealtimeActionOnChatRoomStarted(
    IRealtimeActionPusher realtimeActionPusher,
    IChatQueryService chatQueryService
) : DomainEventHandler<ChatRoomStartedDomainEvent>
{
    private readonly IRealtimeActionPusher _realtimeActionPusher = realtimeActionPusher;
    private readonly IChatQueryService _chatQueryService = chatQueryService;

    public override async Task Handle(
        ChatRoomStartedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var participants = await _chatQueryService.GetParticipantAgentsAsync(
            domainEvent.ChatRoomId,
            cancellationToken
        );

        if (participants.Count != 2)
        {
            throw CorruptedInvariantException.InvalidChatRoomParticipantCount(
                domainEvent.ChatRoomId,
                participants.Count
            );
        }

        var initiatorParticipant = participants.SingleOrDefault(p =>
            p.AgentId != domainEvent.RecipientAgentId
        );

        var recipientParticipant = participants.SingleOrDefault(p =>
            p.AgentId == domainEvent.RecipientAgentId
        );

        if (initiatorParticipant is null || recipientParticipant is null)
        {
            throw CorruptedInvariantException.DuplicateChatRoomParticipants(domainEvent.ChatRoomId);
        }

        await _realtimeActionPusher.PushToAgentAsync(
            agentId: recipientParticipant.AgentId,
            agentTypeId: recipientParticipant.AgentTypeId,
            action: ChatRealtimeActions.ChatRoomStarted,
            payload: new ChatRoomCreatedDto
            {
                Id = domainEvent.ChatRoomId,
                Recipient = initiatorParticipant,
            },
            cancellationToken: cancellationToken
        );

        if (initiatorParticipant.AgentTypeId == AgentType.Company.Id)
        {
            await _realtimeActionPusher.PushToAgentAsync(
                agentId: initiatorParticipant.AgentId,
                agentTypeId: initiatorParticipant.AgentTypeId,
                action: ChatRealtimeActions.ChatRoomStarted,
                payload: new ChatRoomCreatedDto
                {
                    Id = domainEvent.ChatRoomId,
                    Recipient = recipientParticipant,
                },
                cancellationToken: cancellationToken
            );
        }
    }
}
