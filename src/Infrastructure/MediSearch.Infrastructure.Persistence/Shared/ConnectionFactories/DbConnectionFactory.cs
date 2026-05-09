using MediSearch.Shared.Constants;

namespace MediSearch.Infrastructure.Persistence.Shared.ConnectionFactories;

internal sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public DbConnectionFactory(IConfiguration configuration)
    {
        string connection = Guard.Against.NullOrWhiteSpace(
            configuration.GetConnectionString(ServiceNames.Database),
            $"Connection string '{ServiceNames.Database}' is missing or empty."
        );

        _dataSource = NpgsqlDataSource.Create(connection);
    }

    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        return await _dataSource.OpenConnectionAsync();
    }
}
