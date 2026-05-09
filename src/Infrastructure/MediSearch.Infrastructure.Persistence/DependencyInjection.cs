using System.Reflection;
using MediSearch.Core.Application.Shared.Markers;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Core.Domain.SharedKernel.Interfaces;
using MediSearch.Infrastructure.Persistence;
using MediSearch.Infrastructure.Persistence.Shared.Interceptors;
using MediSearch.Shared.Constants;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        string connectionString = Guard.Against.NullOrWhiteSpace(
            builder.Configuration.GetConnectionString(ServiceNames.Database),
            $"Connection string '{ServiceNames.Database}' is missing or empty."
        );

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        builder.Services.AddDbContext<AppDbContext>(
            (sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
                options.ConfigureWarnings(w =>
                    w.Ignore(RelationalEventId.PendingModelChangesWarning)
                );
            }
        );

        builder.EnrichNpgsqlDbContext<AppDbContext>();

        builder.Services.AddScoped<AppDbContextInitialiser>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        builder
            .Services.AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.Scan(scan =>
            scan.FromAssemblies(AssemblyReference.Assembly)
                .AddClasses(c => c.AssignableTo<IRepository>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        builder.Services.Scan(scan =>
            scan.FromAssemblies(AssemblyReference.Assembly)
                .AddClasses(c => c.AssignableTo<IQueryService>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}
