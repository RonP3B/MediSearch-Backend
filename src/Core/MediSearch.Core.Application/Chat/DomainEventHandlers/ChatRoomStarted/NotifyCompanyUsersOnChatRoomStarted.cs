using MediSearch.Core.Application.Chat.Notifications.ChatRoomStarted;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Core.Application.Chat.DomainEventHandlers.ChatRoomStarted;

public sealed class NotifyCompanyUsersOnChatRoomStarted(
    IChatQueryService chatQueryService,
    IUserQueryService userQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<ChatRoomStartedDomainEvent>
{
    private readonly IChatQueryService _chatQueryService = chatQueryService;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ChatRoomStartedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        if (domainEvent.RecipientAgentTypeId != AgentType.Company.Id)
        {
            return;
        }

        var participantNames = await _chatQueryService.GetChatRoomParticipantNamesAsync(
            domainEvent.ChatRoomId,
            domainEvent.RecipientAgentId,
            cancellationToken
        );

        UserContactInfoDto[] companyUsersContactInfo =
        [
            .. await _userQueryService.GetCompanyUsersContactInfoAsync(
                domainEvent.RecipientAgentId,
                cancellationToken
            ),
        ];

        var notifications = companyUsersContactInfo
            .Chunk(_emailService.MaxBatchSize)
            .Select(
                (chunk, index) =>
                    new ChatRoomStartedNotification(
                        participantNames.CompanyName,
                        participantNames.CounterpartyName,
                        chunk,
                        $"{domainEvent.Id}:chat-room-started:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
