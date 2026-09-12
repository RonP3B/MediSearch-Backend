using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.AccessControl;
using MediSearch.Core.Domain.Users.ValueObjects;
using EmailVO = MediSearch.Core.Domain.SharedKernel.ValueObjects.Email;

namespace MediSearch.Infrastructure.Persistence.Shared.Contexts;

public sealed class AppDbContextInitialiser(
    ILogger<AppDbContextInitialiser> logger,
    AppDbContext dbContext,
    IAccountManager accountManager,
    IUserRepository userRepository,
    IConfiguration configuration
)
{
    private readonly ILogger<AppDbContextInitialiser> _logger = logger;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IAccountManager _accountManager = accountManager;
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

    /// <summary>
    /// Makes sure the system administrator exists on both sides of the identity boundary:
    /// the account in the external identity provider (Keycloak) and the domain user row
    /// that points at it through its external id.
    /// </summary>
    private async Task EnsureAdminUserExistsAsync()
    {
        const string adminUsername = "administrator";
        const string adminEmail = "administrator@system.local";
        const string adminPhoneNumber = "(123) 456-7890";

        string? externalUserId = await _accountManager.FindExternalUserIdByUsernameOrDefaultAsync(
            adminUsername
        );

        User? domainUser = await _userRepository.GetByUsernameOrDefaultAsync(
            Username.From(adminUsername)
        );

        if (externalUserId is null)
        {
            externalUserId = await CreateAdminAccountAsync(
                adminUsername,
                adminEmail,
                adminPhoneNumber
            );

            _logger.LogInformation("Administrator identity account created.");
        }

        if (domainUser is null)
        {
            domainUser = User.Create(
                externalId: ExternalId.From(externalUserId),
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

    private async Task<string> CreateAdminAccountAsync(
        string username,
        string email,
        string phoneNumber
    )
    {
        string adminPassword = Guard.Against.NullOrWhiteSpace(
            _configuration["AdminPassword"],
            message: "AdminPassword configuration value is missing or empty."
        );

        var result = await _accountManager.RegisterUserAsync(
            new RegisterExternalUserDto
            {
                Username = username,
                Password = adminPassword,
                Email = email,
                PhoneNumber = phoneNumber,
                IsActive = true,
            }
        );

        if (!result.Succeeded)
        {
            string errorCodes = string.Join(
                ", ",
                result.Errors.SelectMany(error => error.Value).Select(code => code.Key)
            );

            throw new InvalidOperationException(
                $"Failed to create the administrator identity account: {errorCodes}"
            );
        }

        return result.Value.Id;
    }
}
