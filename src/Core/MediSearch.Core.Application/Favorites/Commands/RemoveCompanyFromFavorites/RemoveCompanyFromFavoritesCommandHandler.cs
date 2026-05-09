using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Favorites.CompanyFavorites;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.RemoveCompanyFromFavorites;

public sealed class RemoveCompanyFromFavoritesCommandHandler(
    ICurrentUser currentUser,
    ICompanyFavoriteRepository companyFavoriteRepository
) : ICommandHandler<RemoveCompanyFromFavoritesCommand>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ICompanyFavoriteRepository _companyFavoriteRepository =
        companyFavoriteRepository;

    public async Task Handle(
        RemoveCompanyFromFavoritesCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        var companyFavorite = await _companyFavoriteRepository.GetByCompanyAndAgentOrDefaultAsync(
            EntityId<Company>.From(cmd.CompanyId),
            currentAgent,
            cancellationToken
        );

        if (companyFavorite == null)
        {
            return;
        }

        _companyFavoriteRepository.Remove(companyFavorite);
    }
}
