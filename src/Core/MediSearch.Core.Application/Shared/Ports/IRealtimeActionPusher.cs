namespace MediSearch.Core.Application.Shared.Ports;

public interface IRealtimeActionPusher
{
    Task PushToAgentAsync<T>(
        Guid agentId,
        int agentTypeId,
        string action,
        T payload,
        CancellationToken cancellationToken = default
    );

    Task PushToGroupAsync<T>(
        string groupId,
        string action,
        T payload,
        CancellationToken cancellationToken = default
    );

    Task PushToUserAsync<T>(
        string userId,
        string action,
        T payload,
        CancellationToken cancellationToken = default
    );
}
