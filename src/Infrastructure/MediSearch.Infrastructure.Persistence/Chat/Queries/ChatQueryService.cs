using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Application.Shared.DTOs;
using MediSearch.Core.Domain.SharedKernel.Enums;
using MediSearch.Infrastructure.Persistence.Shared.Exceptions;

namespace MediSearch.Infrastructure.Persistence.Chat.Queries;

internal sealed class ChatQueryService(IDbConnectionFactory db) : IChatQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<IReadOnlyList<ChatListItemDto>> GetChatRoomListAsync(
        Guid currentAgentId,
        int currentAgentTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var chats = await conn.QueryAsync<ChatListItemDto, AgentSummaryDto, ChatListItemDto>(
            ChatSql.GetChatRoomList,
            (chat, recipient) => chat with { Recipient = recipient },
            new
            {
                CurrentAgentId = currentAgentId,
                CurrentAgentTypeId = currentAgentTypeId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            },
            splitOn: nameof(AgentSummaryDto.AgentId)
        );

        return [.. chats];
    }

    public async Task<IReadOnlyList<AgentSummaryDto>> GetParticipantAgentsAsync(
        Guid chatRoomId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var participants = await conn.QueryAsync<AgentSummaryDto>(
            ChatSql.GetParticipantAgents,
            new
            {
                ChatRoomId = chatRoomId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        return [.. participants];
    }

    public async Task<IReadOnlyList<Guid>> GetParticipantChatRoomIdsAsync(
        Guid participantId,
        int participantTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var chatRoomIds = await conn.QueryAsync<Guid>(
            ChatSql.GetParticipantChatRoomIds,
            new { ParticipantId = participantId, ParticipantTypeId = participantTypeId }
        );

        return [.. chatRoomIds];
    }

    public async Task<ChatRoomParticipantNamesDto> GetChatRoomParticipantNamesAsync(
        Guid chatRoomId,
        Guid recipientCompanyId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<ChatRoomParticipantNamesDto>(
            ChatSql.GetChatRoomParticipantNames,
            new
            {
                ChatRoomId = chatRoomId,
                RecipientCompanyId = recipientCompanyId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"Chat room participant names for chat room '{chatRoomId}'"
                    + $" and recipient company '{recipientCompanyId}' were expected"
                    + $" to exist but were not found."
            );
        }

        return result;
    }

    public async Task<ChatRoomMessagesDto?> GetChatRoomMessagesOrDefaultAsync(
        Guid chatRoomId,
        Guid currentAgentId,
        int currentAgentTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        await using var multi = await conn.QueryMultipleAsync(
            ChatSql.GetChatRoomMessages,
            new
            {
                ChatRoomId = chatRoomId,
                CurrentAgentId = currentAgentId,
                CurrentAgentTypeId = currentAgentTypeId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        var recipient = await multi.ReadSingleOrDefaultAsync<AgentSummaryDto>();

        if (recipient is null)
        {
            return null;
        }

        return new ChatRoomMessagesDto
        {
            Id = chatRoomId,
            Recipient = recipient,
            Messages =
            [
                .. multi.Read<MessageDto, AgentSummaryDto, MessageDto>(
                    (message, sender) => message with { Sender = sender },
                    splitOn: nameof(AgentSummaryDto.AgentId)
                ),
            ],
        };
    }
}
