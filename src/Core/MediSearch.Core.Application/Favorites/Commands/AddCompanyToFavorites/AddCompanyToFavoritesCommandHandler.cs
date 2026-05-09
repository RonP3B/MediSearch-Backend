using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Favorites.CompanyFavorites;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Commands.AddCompanyToFavorites;

public sealed class AddCompanyToFavoritesCommandHandler(
    ICurrentUser currentUser,
    ICompanyFavoriteRepository companyFavoriteRepository
) : ICommandHandler<AddCompanyToFavoritesCommand>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ICompanyFavoriteRepository _companyFavoriteRepository =
        companyFavoriteRepository;

    public async Task Handle(AddCompanyToFavoritesCommand cmd, CancellationToken cancellationToken)
    {
        Agent currentAgent = _currentUser.ToAgent();

        var companyId = EntityId<Company>.From(cmd.CompanyId);

        var existing = await _companyFavoriteRepository.GetByCompanyAndAgentOrDefaultAsync(
            companyId,
            currentAgent,
            cancellationToken
        );

        if (existing != null)
        {
            return;
        }

        _companyFavoriteRepository.Add(CompanyFavorite.Create(currentAgent, companyId));
    }
}
