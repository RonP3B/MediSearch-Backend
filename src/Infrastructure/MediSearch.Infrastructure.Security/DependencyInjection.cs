namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    public static void AddSecurityServices(this IHostApplicationBuilder builder)
    {
        builder.AddAuthenticationServices();
        builder.AddAuthorizationServices();
    }
}
