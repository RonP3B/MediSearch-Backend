using FluentValidation;
using MediSearch.Core.Application.Chat.Constants;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Commands.CreateChatRoom;

public sealed class CreateChatRoomCommandValidator : AbstractValidator<CreateChatRoomCommand>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IAgentQueryService _agentQueryService;

    private const string RecipientAgent = nameof(RecipientAgent);

    public CreateChatRoomCommandValidator(
        IChatRoomRepository chatRoomRepository,
        ICurrentUser currentUser,
        IAgentQueryService agentQueryService
    )
    {
        _chatRoomRepository = chatRoomRepository;
        _currentUser = currentUser;
        _agentQueryService = agentQueryService;

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(v => v)
            .ValidValueObject(
                v => Agent.TryFrom(v.RecipientAgentTypeId, v.RecipientAgentId),
                prefix: ChatPrefixes.Recipient
            );

        RuleFor(v => v)
            .MustAsync(RecipientAgentExistsAsync)
            .WithCustomErrorCode(ApplicationErrorCodes.AgentNotFound)
            .OverridePropertyName(RecipientAgent);

        RuleFor(v => v)
            .MustAsync(IsChatAllowed)
            .WithCustomErrorCode(ChatErrorCodes.ChatNotAllowed)
            .OverridePropertyName(RecipientAgent);

        RuleFor(v => v)
            .MustAsync(IsChatRoomUnique)
            .WithCustomErrorCode(ChatErrorCodes.ChatRoomAlreadyExists)
            .OverridePropertyName(RecipientAgent);
    }

    private async Task<bool> RecipientAgentExistsAsync(
        CreateChatRoomCommand cmd,
        CancellationToken cancellationToken
    )
    {
        return await _agentQueryService.AgentExistsAsync(
            cmd.RecipientAgentId,
            cmd.RecipientAgentTypeId,
            cancellationToken
        );
    }

    private async Task<bool> IsChatAllowed(
        CreateChatRoomCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var current = _currentUser.ToAgent();
        var recipient = Agent.From(cmd.RecipientAgentTypeId, cmd.RecipientAgentId);
        return await _chatRoomRepository.IsChatAllowedAsync(current, recipient, cancellationToken);
    }

    private async Task<bool> IsChatRoomUnique(
        CreateChatRoomCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var currentAgent = _currentUser.ToAgent();
        var recipient = Agent.From(cmd.RecipientAgentTypeId, cmd.RecipientAgentId);

        return !await _chatRoomRepository.ExistsByParticipantAgentsAsync(
            currentAgent,
            recipient,
            cancellationToken
        );
    }
}
