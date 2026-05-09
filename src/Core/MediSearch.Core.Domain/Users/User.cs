using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Users.AccessControl;
using MediSearch.Core.Domain.Users.DomainEvents;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Domain.Users;

public sealed class User : BaseAuditableEntity
{
    private readonly List<Role> _roles = [];

    private User() { }

    public EntityId<User> Id { get; private set; } = EntityId<User>.Empty;
    public ExternalId ExternalId { get; private set; } = ExternalId.Empty;
    public FullName FullName { get; private set; } = FullName.Empty;
    public Username Username { get; private set; } = Username.Empty;
    public Email Email { get; private set; } = Email.Empty;
    public PhoneNumber PhoneNumber { get; private set; } = PhoneNumber.Empty;
    public Location Location { get; private set; } = Location.Empty;
    public AssetKey? ProfileImageKey { get; private set; }
    public EntityId<Company>? CompanyId { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public static User Create(
        ExternalId externalId,
        FullName fullName,
        Username username,
        Email email,
        PhoneNumber phoneNumber,
        Location location,
        IEnumerable<Role> roles,
        AssetKey? profileImageKey = null,
        EntityId<Company>? companyId = null
    )
    {
        if (roles is null || !roles.Any())
        {
            throw new BusinessRuleException(nameof(Roles), UserErrorCodes.MustHaveAtLeastOneRole);
        }

        var userId = EntityId<User>.New();

        var user = new User
        {
            Id = userId,
            ExternalId = externalId,
            FullName = fullName,
            Username = username,
            Email = email,
            PhoneNumber = phoneNumber,
            Location = location,
            ProfileImageKey = profileImageKey,
            CompanyId = companyId,
        };

        user._roles.AddRange(roles);

        user.AddDomainEvent(new UserCreatedDomainEvent(user.Id, user.Username));

        return user;
    }

    public void UpdateProfileDetails(FullName fullName, Location location, PhoneNumber phoneNumber)
    {
        FullName = fullName;
        Location = location;
        PhoneNumber = phoneNumber;
    }

    public void UpdateProfileImage(AssetKey? profileImageKey)
    {
        var previousImageKey = ProfileImageKey;
        ProfileImageKey = profileImageKey;

        if (previousImageKey is not null && previousImageKey != profileImageKey)
        {
            AddDomainEvent(new UserProfileImageChangedDomainEvent(Id, previousImageKey));
        }
    }

    public void AddRole(Role role)
    {
        if (_roles.Any(r => r.Id == role.Id))
        {
            throw new BusinessRuleException(nameof(Roles), UserErrorCodes.RoleAlreadyAssigned);
        }

        _roles.Add(role);
    }

    public void RemoveRole(Role role)
    {
        if (!_roles.Any(r => r.Id == role.Id))
        {
            throw new BusinessRuleException(nameof(Roles), UserErrorCodes.RoleNotAssigned);
        }

        if (_roles.Count == 1)
        {
            throw new BusinessRuleException(nameof(Roles), UserErrorCodes.MustHaveAtLeastOneRole);
        }

        _roles.Remove(role);
    }

    public void RemoveCompanyUser()
    {
        if (CompanyId is null)
        {
            throw new BusinessRuleException(
                nameof(User),
                UserErrorCodes.OnlyCompanyUsersCanBeDeleted
            );
        }

        if (_roles.Any(r => r.Id == Role.CompanyOwner.Id))
        {
            throw new BusinessRuleException(
                nameof(User),
                UserErrorCodes.CompanyOwnerCannotBeDeleted
            );
        }

        AddDomainEvent(
            new CompanyUserRemovedDomainEvent(
                Id,
                FullName.ToString(),
                Username,
                CompanyId,
                Email,
                ProfileImageKey?.Key,
                ExternalId
            )
        );
    }
}
