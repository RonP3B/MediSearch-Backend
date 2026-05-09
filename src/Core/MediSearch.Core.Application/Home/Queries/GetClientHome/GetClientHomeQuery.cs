using MediSearch.Core.Application.Home.DTOs;

namespace MediSearch.Core.Application.Home.Queries.GetClientHome;

[Authorize(Roles = nameof(Role.Client))]
public sealed record GetClientHomeQuery : IQuery<ClientHomeDto>;
