using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Domain.SharedKernel.Enums;
using Microsoft.AspNetCore.SignalR;

namespace MediSearch.Presentation.WebApi.Shared.SignalR;

internal sealed class SignalRRealtimeActionPusher(IHubContext<AppHub> hubContext)
    : IRealtimeActionPusher
{
    private readonly IHubContext<AppHub> _hubContext = hubContext;

    public async Task PushToAgentAsync<T>(
        Guid agentId,
        int agentTypeId,
        string actionName,
        T payload,
        CancellationToken cancellationToken = default
    )
    {
        if (agentTypeId == AgentType.User.Id)
        {
            await PushToUserAsync(agentId.ToString(), actionName, payload, cancellationToken);

            return;
        }

        if (agentTypeId == AgentType.Company.Id)
        {
            await PushToGroupAsync(
                SignalRGroupNames.Company(agentId),
                actionName,
                payload,
                cancellationToken
            );

            return;
        }

        throw new ArgumentOutOfRangeException(nameof(agentTypeId), agentTypeId, null);
    }

    public async Task PushToUserAsync<T>(
        string userId,
        string actionName,
        T payload,
        CancellationToken cancellationToken = default
    )
    {
        await _hubContext.Clients.User(userId).SendAsync(actionName, payload, cancellationToken);
    }

    public async Task PushToGroupAsync<T>(
        string groupId,
        string actionName,
        T payload,
        CancellationToken cancellationToken = default
    )
    {
        await _hubContext.Clients.Group(groupId).SendAsync(actionName, payload, cancellationToken);
    }
}
