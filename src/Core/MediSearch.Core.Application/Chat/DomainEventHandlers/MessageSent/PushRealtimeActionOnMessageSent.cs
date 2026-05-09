using MediSearch.Core.Application.Chat.Constants;
using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Domain.Chat.Messages;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Core.Application.Chat.DomainEventHandlers.MessageSent;

public sealed class PushRealtimeActionOnMessageSent(
    IRealtimeActionPusher realtimeActionPusher,
    IChatQueryService chatQueryService,
    ICacheService cacheService
) : DomainEventHandler<MessageSentDomainEvent>
{
    private readonly IRealtimeActionPusher _realtimeActionPusher = realtimeActionPusher;
    private readonly IChatQueryService _chatQueryService = chatQueryService;
    private readonly ICacheService _cacheService = cacheService;

    public override async Task Handle(
        MessageSentDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var participants = await GetCachedParticipantAgentsAsync(
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

        var senderParticipant = participants.SingleOrDefault(p =>
            p.AgentId == domainEvent.SenderAgentId
        );

        var recipientParticipant = participants.SingleOrDefault(p =>
            p.AgentId != domainEvent.SenderAgentId
        );

        if (senderParticipant is null || recipientParticipant is null)
        {
            throw CorruptedInvariantException.DuplicateChatRoomParticipants(domainEvent.ChatRoomId);
        }

        var messageDto = new MessageDto
        {
            Id = domainEvent.MessageId,
            ChatRoomId = domainEvent.ChatRoomId,
            MessageSentDate = domainEvent.SentDate,
            TextContent = domainEvent.TextContent,
            MediaContentAssetKey = domainEvent.MediaContentAssetKey,
            MediaContentType = domainEvent.MediaContentType,
            Sender = senderParticipant,
        };

        await _realtimeActionPusher.PushToAgentAsync(
            agentId: recipientParticipant.AgentId,
            agentTypeId: recipientParticipant.AgentTypeId,
            action: ChatRealtimeActions.MessageSent,
            payload: messageDto,
            cancellationToken: cancellationToken
        );

        if (senderParticipant.AgentTypeId == AgentType.Company.Id)
        {
            await _realtimeActionPusher.PushToAgentAsync(
                agentId: senderParticipant.AgentId,
                agentTypeId: senderParticipant.AgentTypeId,
                action: ChatRealtimeActions.MessageSent,
                payload: messageDto,
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task<IReadOnlyList<AgentSummaryDto>> GetCachedParticipantAgentsAsync(
        Guid chatRoomId,
        CancellationToken cancellationToken
    )
    {
        string cacheKey = ChatCacheKeys.ParticipantAgents(chatRoomId);

        var cached = await _cacheService.GetAsync<IReadOnlyList<AgentSummaryDto>>(
            cacheKey,
            cancellationToken
        );

        if (cached is not null)
        {
            return cached;
        }

        var participants = await _chatQueryService.GetParticipantAgentsAsync(
            chatRoomId,
            cancellationToken
        );

        await _cacheService.SetAsync(
            cacheKey,
            participants,
            TimeSpan.FromHours(6),
            cancellationToken
        );

        return participants;
    }
}
