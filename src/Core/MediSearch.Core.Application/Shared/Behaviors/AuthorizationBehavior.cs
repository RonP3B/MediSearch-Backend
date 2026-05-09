using System.Reflection;
using MediatR;

namespace MediSearch.Core.Application.Shared.Behaviors;

internal sealed class AuthorizationBehavior<TRequest, TResponse>(
    ICurrentUser currentUser,
    IPermissionService permissionService
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IPermissionService _permissionService = permissionService;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        Type requestType = request.GetType();

        IEnumerable<AuthorizeAttribute> authorizeAttributes =
            requestType.GetCustomAttributes<AuthorizeAttribute>();

        if (!authorizeAttributes.Any())
        {
            // Authorization is not required
            return await next(cancellationToken);
        }

        if (_currentUser.Id == null)
        {
            throw new UnauthorizedException();
        }

        ValidateRoles(authorizeAttributes);

        await ValidatePermissionsAsync(authorizeAttributes, cancellationToken);

        // User is authorized
        return await next(cancellationToken);
    }

    private void ValidateRoles(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        var requiredRoles = new HashSet<string>(
            authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
                .SelectMany(a => a.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(r => r.Trim()),
            StringComparer.OrdinalIgnoreCase
        );

        if (requiredRoles.Count == 0)
        {
            return;
        }

        if (_currentUser.Roles.Any(roles => requiredRoles.Contains(roles)))
        {
            return;
        }

        throw new ForbiddenAccessException();
    }

    private async Task ValidatePermissionsAsync(
        IEnumerable<AuthorizeAttribute> authorizeAttributes,
        CancellationToken cancellationToken
    )
    {
        List<string> requiredPermissions =
        [
            .. authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Permission))
                .Select(a => a.Permission)
                .Distinct(),
        ];

        foreach (string permission in requiredPermissions)
        {
            bool isGranted = await _permissionService.IsPermissionGrantedByRolesAsync(
                _currentUser.Roles,
                permission,
                cancellationToken
            );

            if (!isGranted)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}
