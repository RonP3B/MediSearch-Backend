using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.Enums;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Chat.Repositories;

internal sealed class ChatRoomRepository(AppDbContext dbContext) : IChatRoomRepository
{
    public async Task<ChatRoom?> GetByIdOrDefaultAsync(
        EntityId<ChatRoom> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.ChatRooms.SingleOrDefaultAsync(cr => cr.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByParticipantAgentsAsync(
        Agent participantAgentA,
        Agent participantAgentB,
        CancellationToken cancellationToken
    )
    {
        return await dbContext.ChatRooms.AnyAsync(
            cr =>
                cr.Participants.Any(p =>
                    p.Agent.AgentTypeId == participantAgentA.AgentTypeId
                    && p.Agent.AgentId == participantAgentA.AgentId
                )
                && cr.Participants.Any(p =>
                    p.Agent.AgentTypeId == participantAgentB.AgentTypeId
                    && p.Agent.AgentId == participantAgentB.AgentId
                ),
            cancellationToken
        );
    }

    public async Task<bool> IsChatAllowedAsync(
        Agent participantAgentA,
        Agent participantAgentB,
        CancellationToken cancellationToken
    )
    {
        var companyIds = new[] { participantAgentA, participantAgentB }
            .Where(a => a.AgentTypeId == AgentType.Company.Id)
            .Select(a => a.AgentId)
            .ToList();

        if (companyIds.Count == 0)
        {
            return false;
        }

        var pharmacyCount = await dbContext
            .Companies.Where(c => companyIds.Contains(c.Id))
            .CountAsync(c => c.CompanyTypeId == CompanyType.Pharmacy.Id, cancellationToken);

        return pharmacyCount == 1;
    }

    public void Add(ChatRoom chatRoom)
    {
        dbContext.ChatRooms.Add(chatRoom);
    }
}
