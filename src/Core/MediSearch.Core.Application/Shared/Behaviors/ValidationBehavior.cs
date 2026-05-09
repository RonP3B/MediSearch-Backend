using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Hosting;
using ValidationException = MediSearch.Core.Application.Shared.Exceptions.ValidationException;

namespace MediSearch.Core.Application.Shared.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger,
    IHostEnvironment env
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IHostEnvironment _env = env;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (_validators.Any())
        {
            var context = ValidationContext<TRequest>.CreateWithOptions(
                request,
                options => options.IncludeAllRuleSets()
            );

            ValidationResult[] validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            List<ValidationFailure> failures =
            [
                .. validationResults.Where(r => r.Errors.Count != 0).SelectMany(r => r.Errors),
            ];

            if (failures.Count > 0)
            {
                var (ex, propertiesMissingErrorCodes) = ValidationException.FromFailures(failures);

                if (propertiesMissingErrorCodes.Count > 0)
                {
                    if (_env.IsDevelopment())
                    {
                        throw new InvalidOperationException(
                            $"Validation rules missing ErrorCode for properties: {string.Join(", ", propertiesMissingErrorCodes)}"
                        );
                    }

                    foreach (var property in propertiesMissingErrorCodes)
                    {
                        _logger.LogError(
                            "Validation rule missing ErrorCode for property {Property}",
                            property
                        );
                    }
                }

                throw ex;
            }
        }

        return await next(cancellationToken);
    }
}
