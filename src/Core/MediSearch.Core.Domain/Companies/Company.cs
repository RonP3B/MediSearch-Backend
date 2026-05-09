using MediSearch.Core.Domain.Companies.DomainEvents;
using MediSearch.Core.Domain.Companies.ValueObjects;

namespace MediSearch.Core.Domain.Companies;

public sealed class Company : BaseAuditableEntity
{
    private Company() { }

    public EntityId<Company> Id { get; private set; } = EntityId<Company>.Empty;
    public CompanyName Name { get; private set; } = CompanyName.Empty;
    public CompanyCeoName CeoName { get; private set; } = CompanyCeoName.Empty;
    public Location Location { get; private set; } = Location.Empty;
    public AssetKey ImageKey { get; private set; } = AssetKey.Empty;
    public Email Email { get; private set; } = Email.Empty;
    public PhoneNumber PhoneNumber { get; private set; } = PhoneNumber.Empty;
    public int CompanyTypeId { get; private set; }

    public Url? Website { get; private set; }
    public Url? Facebook { get; private set; }
    public Url? Instagram { get; private set; }
    public Url? Twitter { get; private set; }

    public static Company Create(
        CompanyName name,
        CompanyCeoName ceoName,
        Location location,
        AssetKey imageKey,
        Email email,
        PhoneNumber phoneNumber,
        CompanyType companyType,
        Url? website = null,
        Url? facebook = null,
        Url? instagram = null,
        Url? twitter = null
    )
    {
        var companyId = EntityId<Company>.New();

        var company = new Company
        {
            Id = companyId,
            Name = name,
            CeoName = ceoName,
            Location = location,
            ImageKey = imageKey,
            Email = email,
            PhoneNumber = phoneNumber,
            CompanyTypeId = companyType.Id,
            Website = website,
            Facebook = facebook,
            Instagram = instagram,
            Twitter = twitter,
        };

        company.AddDomainEvent(new CompanyCreatedDomainEvent(companyId));

        return company;
    }

    public void UpdateDetails(
        CompanyName name,
        CompanyCeoName ceoName,
        Location location,
        Email email,
        PhoneNumber phoneNumber,
        AssetKey imageKey,
        Url? website,
        Url? facebook,
        Url? instagram,
        Url? twitter
    )
    {
        CeoName = ceoName;
        Location = location;
        Email = email;
        PhoneNumber = phoneNumber;
        Website = website;
        Facebook = facebook;
        Instagram = instagram;
        Twitter = twitter;
        UpdateName(name);
        UpdateImageKey(imageKey);
    }

    private void UpdateName(CompanyName name)
    {
        if (Name == name)
        {
            return;
        }

        var previousName = Name;
        Name = name;
        AddDomainEvent(new CompanyRenamedDomainEvent(Id, previousName, name));
    }

    private void UpdateImageKey(AssetKey imageKey)
    {
        if (ImageKey == imageKey)
        {
            return;
        }

        var previousImageKey = ImageKey;
        ImageKey = imageKey;
        AddDomainEvent(new CompanyLogoChangedDomainEvent(Id, Name, previousImageKey, imageKey));
    }
}
