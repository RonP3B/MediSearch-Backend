using MediSearch.Core.Domain.SharedKernel.Enums;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Shared.Extensions;

internal static class CurrentUserExtensions
{
    /// <summary>
    /// Gets the authenticated user's ID or throws if the user is not authenticated.
    /// </summary>
    /// <param name="currentUser">The current user context.</param>
    /// <returns>The authenticated user's ID.</returns>
    /// <exception cref="UnauthorizedException">Thrown when the user is not authenticated.</exception>
    public static Guid GetAuthenticatedUserId(this ICurrentUser currentUser)
    {
        return currentUser.Id ?? throw new UnauthorizedException();
    }

    /// <summary>
    /// Gets the authenticated user's external ID or throws if the user is not authenticated.
    /// </summary>
    /// <param name="currentUser">The current user context.</param>
    /// <returns>The authenticated user's external ID.</returns>
    /// <exception cref="UnauthorizedException">Thrown when the user is not authenticated.</exception>
    public static string GetAuthenticatedExternalUserId(this ICurrentUser currentUser)
    {
        return currentUser.ExternalId ?? throw new UnauthorizedException();
    }

    /// <summary>
    /// Gets the authenticated user's company ID or throws if not authenticated or not associated with a company.
    /// </summary>
    /// <param name="currentUser">The current user context.</param>
    /// <returns>The authenticated user's company ID.</returns>
    public static Guid GetAuthenticatedUserCompanyId(this ICurrentUser currentUser)
    {
        if (currentUser.Id is null)
        {
            throw new UnauthorizedException();
        }

        return currentUser.CompanyId ?? throw new ForbiddenAccessException();
    }

    /// <summary>
    /// Converts the current user to an <see cref="Agent"/> value object.
    /// </summary>
    /// <param name="currentUser">The current authenticated user context.</param>
    /// <returns>
    /// An <see cref="Agent"/> representing the company or user associated
    /// with the current session.
    /// </returns>
    public static Agent ToAgent(this ICurrentUser currentUser)
    {
        if (currentUser.Id is null)
        {
            throw new UnauthorizedException();
        }

        if (currentUser.CompanyId.HasValue)
        {
            return Agent.From(AgentType.Company.Id, currentUser.CompanyId.Value);
        }

        return Agent.From(AgentType.User.Id, currentUser.Id.Value);
    }

    /// <summary>
    /// Converts the current user to an <see cref="Agent"/> value object or returns null
    /// if the user is not authenticated.
    /// </summary>
    /// <param name="currentUser">The current user context.</param>
    /// <returns>
    /// An <see cref="Agent"/> representing the company or user associated with the current session,
    /// or null if the user is not authenticated.
    /// </returns>
    public static Agent? ToAgentOrNull(this ICurrentUser currentUser)
    {
        if (currentUser.Id is null)
        {
            return null;
        }

        if (currentUser.CompanyId.HasValue)
        {
            return Agent.From(AgentType.Company.Id, currentUser.CompanyId.Value);
        }

        return Agent.From(AgentType.User.Id, currentUser.Id.Value);
    }
}
