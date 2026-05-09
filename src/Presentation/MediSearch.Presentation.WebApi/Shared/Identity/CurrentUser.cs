using System.Security.Claims;
using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Shared.Ports;

namespace MediSearch.Presentation.WebApi.Shared.Identity;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? Id => GetGuidClaim(ClaimTypes.NameIdentifier);

    public string? ExternalId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(CustomClaimTypes.ExternalUserId);

    public Guid? CompanyId => GetGuidClaim(CustomClaimTypes.CompanyId);

    public IReadOnlyList<string> Roles =>
        _httpContextAccessor
            .HttpContext?.User?.Claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? [];

    private Guid? GetGuidClaim(string claimType)
    {
        string? value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
        return Guid.TryParse(value, out var guid) ? guid : null;
    }
}
