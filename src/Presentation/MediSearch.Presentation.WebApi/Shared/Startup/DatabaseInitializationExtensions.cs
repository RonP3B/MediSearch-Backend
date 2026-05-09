using MediSearch.Infrastructure.Persistence.Shared.Contexts;

namespace MediSearch.Presentation.WebApi.Shared.Startup;

internal static class DatabaseInitializationExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        AppDbContextInitialiser initialiser =
            scope.ServiceProvider.GetRequiredService<AppDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}
