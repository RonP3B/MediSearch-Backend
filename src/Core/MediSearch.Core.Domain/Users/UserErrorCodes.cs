namespace MediSearch.Core.Domain.Users;

internal static class UserErrorCodes
{
    public const string FirstNameTooLong = "Domain.Users.FirstNameTooLong";
    public const string LastNameTooLong = "Domain.Users.LastNameTooLong";
    public const string FirstNameTooShort = "Domain.Users.FirstNameTooShort";
    public const string LastNameTooShort = "Domain.Users.LastNameTooShort";
    public const string InvalidFirstName = "Domain.Users.InvalidFirstName";
    public const string InvalidLastName = "Domain.Users.InvalidLastName";
    public const string UsernameTooShort = "Domain.Users.UsernameTooShort";
    public const string UsernameTooLong = "Domain.Users.UsernameTooLong";
    public const string UsernameInvalidCharacters = "Domain.Users.UsernameInvalidCharacters";
    public const string MustHaveAtLeastOneRole = "Domain.Users.MustHaveAtLeastOneRole";
    public const string RoleAlreadyAssigned = "Domain.Users.RoleAlreadyAssigned";
    public const string RoleNotAssigned = "Domain.Users.RoleNotAssigned";
    public const string CompanyOwnerCannotBeDeleted = "Domain.Users.CompanyOwnerCannotBeDeleted";
    public const string OnlyCompanyUsersCanBeDeleted = "Domain.Users.OnlyCompanyUsersCanBeDeleted";
    public const string ExternalIdTooLong = "Domain.Users.ExternalIdTooLong";
}
