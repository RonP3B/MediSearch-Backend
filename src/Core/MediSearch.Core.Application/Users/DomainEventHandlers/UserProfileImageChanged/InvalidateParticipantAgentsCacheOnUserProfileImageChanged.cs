using MediSearch.Core.Application.Chat.Constants;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Domain.SharedKernel.Enums;
using MediSearch.Core.Domain.Users.DomainEvents;

namespace MediSearch.Core.Application.Users.DomainEventHandlers.UserProfileImageChanged;

public sealed class InvalidateParticipantAgentsCacheOnUserProfileImageChanged(
    IChatQueryService chatQueryService,
    ICacheService cacheService
) : DomainEventHandler<UserProfileImageChangedDomainEvent>
{
    private readonly IChatQueryService _chatQueryService = chatQueryService;
    private readonly ICacheService _cacheService = cacheService;

    public override async Task Handle(
        UserProfileImageChangedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var chatRoomIds = await _chatQueryService.GetParticipantChatRoomIdsAsync(
            domainEvent.UserId,
            AgentType.User.Id,
            cancellationToken
        );

        await _cacheService.RemoveManyAsync(
            chatRoomIds.Select(ChatCacheKeys.ParticipantAgents),
            cancellationToken
        );
    }
}
