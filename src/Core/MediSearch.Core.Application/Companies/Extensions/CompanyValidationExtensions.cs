using FluentValidation;
using MediSearch.Core.Application.Companies.Constants;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Companies.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Companies.Extensions;

internal static class CompanyValidationExtensions
{
    /// <summary>
    /// Validates that a company name is unique by checking the company repository.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <param name="companyRepository">The company repository to check uniqueness.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, string}"/> configured with async uniqueness validation.
    /// </returns>
    public static IRuleBuilderOptions<T, string> UniqueCompanyName<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository companyRepository
    )
    {
        return rule.MustAsync(
                async (companyName, cancellationToken) =>
                    !await companyRepository.IsCompanyNameTakenAsync(
                        CompanyName.From(companyName),
                        cancellationToken
                    )
            )
            .WithCustomErrorCode(CompanyErrorCodes.CompanyNameAlreadyExists);
    }

    /// <summary>
    /// Validates that a company name is unique, excluding the current user's company.
    /// Used in update scenarios to allow a company to retain its current name.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="companyRepository">The company repository to check uniqueness.</param>
    /// <param name="currentUser">The current authenticated user context.</param>
    /// <param name="nameSelector">A function to extract the company name from the request.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, T}"/> configured with async uniqueness validation.
    /// </returns>
    public static IRuleBuilderOptions<T, T> UniqueCompanyName<T>(
        this IRuleBuilder<T, T> ruleBuilder,
        ICompanyRepository companyRepository,
        ICurrentUser currentUser,
        Func<T, string> nameSelector
    )
    {
        return ruleBuilder
            .MustAsync(
                async (cmd, _, cancellationToken) =>
                    !await companyRepository.IsCompanyNameTakenAsync(
                        CompanyName.From(nameSelector(cmd)),
                        EntityId<Company>.From(currentUser.GetAuthenticatedUserCompanyId()),
                        cancellationToken
                    )
            )
            .WithCustomErrorCode(CompanyErrorCodes.CompanyNameAlreadyExists);
    }
}
