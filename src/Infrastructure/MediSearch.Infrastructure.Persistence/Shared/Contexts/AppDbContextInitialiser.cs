using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.AccessControl;
using MediSearch.Core.Domain.Users.ValueObjects;
using EmailVO = MediSearch.Core.Domain.SharedKernel.ValueObjects.Email;

namespace MediSearch.Infrastructure.Persistence.Shared.Contexts;

public sealed class AppDbContextInitialiser(
    ILogger<AppDbContextInitialiser> logger,
    AppDbContext dbContext,
    UserManager<IdentityUser> userManager,
    IUserRepository userRepository,
    IConfiguration configuration
)
{
    private readonly ILogger<AppDbContextInitialiser> _logger = logger;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task InitialiseAsync()
    {
        try
        {
            await _dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await EnsureAdminUserExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task EnsureAdminUserExistsAsync()
    {
        const string adminUsername = "administrator";
        const string adminEmail = "administrator@system.local";
        const string adminPhoneNumber = "(123) 456-7890";

        IdentityUser? identityUser = await _userManager.FindByNameAsync(adminUsername);
        User? domainUser = await _userRepository.GetByUsernameOrDefaultAsync(
            Username.From(adminUsername)
        );

        if (identityUser is null)
        {
            string adminPassword = Guard.Against.NullOrWhiteSpace(
                _configuration["AdminPassword"],
                message: "AdminPassword configuration value is missing or empty."
            );

            identityUser = new IdentityUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                PhoneNumber = adminPhoneNumber,
                EmailConfirmed = true,
            };

            IdentityResult identityResult = await _userManager.CreateAsync(
                identityUser,
                adminPassword
            );

            if (!identityResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create administrator identity user: "
                        + string.Join(", ", identityResult.Errors.Select(e => e.Description))
                );
            }

            _logger.LogInformation("Administrator identity user created.");
        }

        if (domainUser is null)
        {
            domainUser = User.Create(
                externalId: ExternalId.From(identityUser.Id),
                fullName: FullName.From("System", "Administrator"),
                username: Username.From(adminUsername),
                email: EmailVO.From(adminEmail),
                phoneNumber: PhoneNumber.From(adminPhoneNumber),
                location: Location.From("N/A", "N/A", "N/A"),
                roles: [Role.SystemAdmin]
            );

            _userRepository.Add(domainUser);

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Administrator domain user created.");
        }

        _logger.LogInformation("Administrator user ensured.");
    }
}
