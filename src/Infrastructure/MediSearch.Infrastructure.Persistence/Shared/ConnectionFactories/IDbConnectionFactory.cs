namespace MediSearch.Infrastructure.Persistence.Shared.ConnectionFactories;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
