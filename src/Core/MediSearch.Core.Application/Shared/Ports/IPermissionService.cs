namespace MediSearch.Core.Application.Shared.Ports;

public interface IPermissionService
{
    Task<bool> IsPermissionGrantedByRolesAsync(
        IReadOnlyList<string> userRoles,
        string permissionName,
        CancellationToken cancellationToken = default
    );
}
