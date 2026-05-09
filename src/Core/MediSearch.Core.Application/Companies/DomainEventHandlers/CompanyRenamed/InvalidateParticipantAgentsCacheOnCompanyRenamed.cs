using MediSearch.Core.Application.Chat.Constants;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Domain.Companies.DomainEvents;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Core.Application.Companies.DomainEventHandlers.CompanyRenamed;

public sealed class InvalidateParticipantAgentsCacheOnCompanyRenamed(
    IChatQueryService chatQueryService,
    ICacheService cacheService
) : DomainEventHandler<CompanyRenamedDomainEvent>
{
    private readonly IChatQueryService _chatQueryService = chatQueryService;
    private readonly ICacheService _cacheService = cacheService;

    public override async Task Handle(
        CompanyRenamedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var chatRoomIds = await _chatQueryService.GetParticipantChatRoomIdsAsync(
            domainEvent.CompanyId,
            AgentType.Company.Id,
            cancellationToken
        );

        await _cacheService.RemoveManyAsync(
            chatRoomIds.Select(ChatCacheKeys.ParticipantAgents),
            cancellationToken
        );
    }
}
