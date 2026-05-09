using FluentValidation.Results;
using MediSearch.Core.Domain.SharedKernel.Bases;

namespace MediSearch.Core.Application.Shared.Exceptions;

public sealed class ValidationException : Exception
{
    public ValidationException(IDictionary<string, ErrorCode[]> errors)
        : base(ApplicationErrorCodes.ValidationFailed)
    {
        Errors = errors;
    }

    public IDictionary<string, ErrorCode[]> Errors { get; }

    public static (
        ValidationException Exception,
        List<string> PropertiesMissingErrorCodes
    ) FromFailures(IEnumerable<ValidationFailure> failures)
    {
        var propertiesMissingErrorCodes = new List<string>();

        var errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g =>
                    g.Select(f =>
                        {
                            if (f.CustomState is ErrorCode errorCode)
                            {
                                return errorCode;
                            }

                            propertiesMissingErrorCodes.Add($"{f.PropertyName}");

                            return ApplicationErrorCodes.UnknownValidationError;
                        })
                        .ToArray()
            );

        return (new ValidationException(errors), propertiesMissingErrorCodes);
    }
}
