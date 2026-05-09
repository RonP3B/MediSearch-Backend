using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Shared.Results;
using MediSearch.Core.Domain.SharedKernel.Bases;
using MediSearch.Infrastructure.Security.Authentication.Constants;
using Microsoft.AspNetCore.Identity;

namespace MediSearch.Infrastructure.Security.Authentication.AspNetCoreIdentity;

internal static class AspNetCoreIdentityResultExtensions
{
    public static ServiceResult<T> ToServiceResult<T>(this IdentityResult result, T value)
    {
        return result.Succeeded
            ? ServiceResult<T>.Success(value)
            : ServiceResult<T>.Failure(
                result.Errors.Select(e =>
                    (
                        ErrorKey: MapErrorToPropertyName(e.Code),
                        ErrorCode: MapAspNetCoreIdentityError(e)
                    )
                )
            );
    }

    public static ServiceResult ToServiceResult(this IdentityResult result)
    {
        return result.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Failure(
                result.Errors.Select(e =>
                    (
                        ErrorKey: MapErrorToPropertyName(e.Code),
                        ErrorCode: MapAspNetCoreIdentityError(e)
                    )
                )
            );
    }

    private static string MapErrorToPropertyName(string errorCode)
    {
        return errorCode switch
        {
            string c when c.Contains("Token", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Token,

            string c when c.Contains("Email", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Email,

            string c when c.Contains("Password", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Password,

            string c when c.Contains("UserName", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.Username,

            string c when c.Contains("PhoneNumber", StringComparison.OrdinalIgnoreCase) =>
                AuthenticationErrorKeys.PhoneNumber,

            _ => nameof(IdentityResult),
        };
    }

    private static ErrorCode MapAspNetCoreIdentityError(IdentityError error)
    {
        var code = AspNetCoreIdentityErrorCodeMap.Map.TryGetValue(error.Code, out var mapped)
            ? mapped
            : AccountErrorCodes.UnknownAccountError;

        return new ErrorCode(code);
    }
}
