using MediSearch.Core.Application.Accounts.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MediSearch.Presentation.WebApi.Shared.SignalR;

[Authorize]
internal sealed class AppHub : Hub
{
    public const string Route = "/hubs/app";

    public override async Task OnConnectedAsync()
    {
        string? companyId = Context.User?.FindFirst(CustomClaimTypes.CompanyId)?.Value;

        if (Guid.TryParse(companyId, out Guid parsedCompanyId))
        {
            await JoinGroup(SignalRGroupNames.Company(parsedCompanyId));
        }

        await base.OnConnectedAsync();
    }

    public async Task JoinGroup(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }

    public async Task LeaveGroup(string group)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }
}
