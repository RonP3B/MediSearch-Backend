namespace MediSearch.Presentation.WebApi.Shared.SignalR;

internal static class SignalRGroupNames
{
    public static string Company(Guid companyId) => $"company:{companyId}";
}
