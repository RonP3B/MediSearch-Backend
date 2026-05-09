using FluentValidation;
using FluentValidation.Results;
using MediSearch.Core.Application.Users.Constants;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.Bases;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Shared.Extensions;

internal static class SharedValidationExtensions
{
    /// <summary>
    /// Attaches a domain-level <see cref="ErrorCode"/> to a FluentValidation rule.
    /// The error code key is stored in <see cref="ValidationFailure.ErrorCode"/>,
    /// while the full <see cref="ErrorCode"/> (including parameters) is stored in
    /// <see cref="ValidationFailure.CustomState"/> for later reconstruction.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <typeparam name="TProperty">The property type being validated.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <param name="errorCode">The error code to associate with the validation failure.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, TProperty}"/> configured with the given error code.
    /// </returns>
    public static IRuleBuilderOptions<T, TProperty> WithCustomErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        ErrorCode errorCode
    )
    {
        return rule.WithErrorCode(errorCode.Key).WithState(_ => errorCode);
    }

    /// <summary>
    /// Validates that a property is not empty and attaches a required field error code.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <typeparam name="TProperty">The property type being validated.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, TProperty}"/> configured with not empty validation.
    /// </returns>
    public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>(
        this IRuleBuilderInitial<T, TProperty> rule
    )
    {
        return DefaultValidatorExtensions
            .NotEmpty(rule)
            .WithCustomErrorCode(ApplicationErrorCodes.RequiredField);
    }

    /// <summary>
    /// Validates that a property is not null and attaches a required field error code.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <typeparam name="TProperty">The property type being validated.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, TProperty}"/> configured with not null validation.
    /// </returns>
    public static IRuleBuilderOptions<T, TProperty> NotNull<T, TProperty>(
        this IRuleBuilderInitial<T, TProperty> rule
    )
    {
        return DefaultValidatorExtensions
            .NotNull(rule)
            .WithCustomErrorCode(ApplicationErrorCodes.RequiredField);
    }

    /// <summary>
    /// Applies standard password validation rules:
    /// - Minimum length: 8 characters
    /// - Maximum length: 128 characters
    /// - Must contain at least one lowercase letter
    /// - Must contain at least one uppercase letter
    /// - Must contain at least one digit
    /// - Must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?)
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, string}"/> configured with password validation rules.
    /// </returns>
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.MinimumLength(8)
            .WithCustomErrorCode(ApplicationErrorCodes.PasswordTooShort)
            .MaximumLength(128)
            .WithCustomErrorCode(ApplicationErrorCodes.PasswordTooLong)
            .Matches("[a-z]")
            .WithCustomErrorCode(ApplicationErrorCodes.PasswordRequiresLowercase)
            .Matches("[A-Z]")
            .WithCustomErrorCode(ApplicationErrorCodes.PasswordRequiresUppercase)
            .Matches(@"\d")
            .WithCustomErrorCode(ApplicationErrorCodes.PasswordRequiresDigit)
            .Matches(@"[!@#$%^&*()_+\-=\[\]{}|;:,.<>?]")
            .WithCustomErrorCode(ApplicationErrorCodes.PasswordRequiresSpecialCharacter);
    }

    /// <summary>
    /// Validates that an email address is unique by checking the user repository.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <param name="userRepository">The user repository to check uniqueness.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, string}"/> configured with async uniqueness validation.
    /// </returns>
    public static IRuleBuilderOptions<T, string> UniqueEmail<T>(
        this IRuleBuilder<T, string> rule,
        IUserRepository userRepository
    )
    {
        return rule.MustAsync(
                async (email, cancellationToken) =>
                    !await userRepository.IsEmailTakenAsync(Email.From(email), cancellationToken)
            )
            .WithCustomErrorCode(ApplicationErrorCodes.EmailAlreadyExists);
    }

    /// <summary>
    /// Validates that an email address is unique by checking the company repository.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <param name="companyRepository">The company repository to check uniqueness.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, string}"/> configured with async uniqueness validation.
    /// </returns>
    public static IRuleBuilderOptions<T, string> UniqueEmail<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository companyRepository
    )
    {
        return rule.MustAsync(
                async (email, cancellationToken) =>
                    !await companyRepository.IsEmailTakenAsync(Email.From(email), cancellationToken)
            )
            .WithCustomErrorCode(ApplicationErrorCodes.EmailAlreadyExists);
    }

    /// <summary>
    /// Validates that a username is unique by checking the user repository.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <param name="userRepository">The user repository to check uniqueness.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptions{T, string}"/> configured with async uniqueness validation.
    /// </returns>
    public static IRuleBuilderOptions<T, string> UniqueUsername<T>(
        this IRuleBuilder<T, string> rule,
        IUserRepository userRepository
    )
    {
        return rule.MustAsync(
                async (username, cancellationToken) =>
                    !await userRepository.IsUsernameTakenAsync(
                        Username.From(username),
                        cancellationToken
                    )
            )
            .WithCustomErrorCode(UserErrorCodes.UsernameAlreadyExists);
    }

    /// <summary>
    /// Validates a property by attempting to create a value object from it.
    /// If creation fails, validation errors are added to the context with an optional prefix.
    /// </summary>
    /// <typeparam name="T">The request or model type.</typeparam>
    /// <typeparam name="TProperty">The property type being validated.</typeparam>
    /// <typeparam name="TValueObject">The value object type to create.</typeparam>
    /// <param name="rule">The rule builder.</param>
    /// <param name="tryFrom">A function that attempts to create the value object.</param>
    /// <param name="prefix">An optional prefix for error property names.</param>
    /// <returns>
    /// A <see cref="IRuleBuilderOptionsConditions{T, TProperty}"/> configured with value object validation.
    /// </returns>
    public static IRuleBuilderOptionsConditions<T, TProperty> ValidValueObject<
        T,
        TProperty,
        TValueObject
    >(
        this IRuleBuilder<T, TProperty> rule,
        Func<TProperty, Result<TValueObject>> tryFrom,
        string? prefix = null
    )
    {
        return rule.Custom(
            (value, context) =>
            {
                var result = tryFrom(value);

                if (result.IsSuccess)
                {
                    return;
                }

                foreach (var failure in result.Failures)
                {
                    var propertyName = string.IsNullOrWhiteSpace(prefix)
                        ? failure.PropertyName
                        : $"{prefix}{failure.PropertyName}";

                    var validationFailure = new ValidationFailure
                    {
                        CustomState = failure.ErrorCode,
                        PropertyName = string.IsNullOrEmpty(context.PropertyPath)
                            ? propertyName
                            : context.PropertyPath,
                    };

                    context.AddFailure(validationFailure);
                }
            }
        );
    }
}
