using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;
using MediSearch.Presentation.WebApi.Shared.OpenApi;

namespace MediSearch.Presentation.WebApi.Shared.Endpoints;

internal static class IEndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapGet(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = ""
    )
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapGet(pattern, handler).WithName(handler.Method.Name);
    }

    public static RouteHandlerBuilder MapPost(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = ""
    )
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapPost(pattern, handler).WithName(handler.Method.Name);
    }

    public static RouteHandlerBuilder MapPut(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern
    )
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapPut(pattern, handler).WithName(handler.Method.Name);
    }

    public static RouteHandlerBuilder MapPatch(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern
    )
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapPatch(pattern, handler).WithName(handler.Method.Name);
    }

    public static RouteHandlerBuilder MapDelete(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern
    )
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapDelete(pattern, handler).WithName(handler.Method.Name);
    }
}
