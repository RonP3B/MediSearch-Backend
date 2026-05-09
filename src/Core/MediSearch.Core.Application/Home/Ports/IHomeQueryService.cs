using MediSearch.Core.Application.Home.DTOs;

namespace MediSearch.Core.Application.Home.Ports;

public interface IHomeQueryService : IQueryService
{
    Task<PublicHomeDto> GetPublicHomeAsync(CancellationToken cancellationToken);
    Task<ClientHomeDto> GetClientHomeAsync(Guid userId, CancellationToken cancellationToken);
}
