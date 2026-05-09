using System.Reflection;
using MassTransit;
using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.Chat.Messages;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Favorites.CompanyFavorites;
using MediSearch.Core.Domain.Favorites.ProductFavorites;
using MediSearch.Core.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MediSearch.Infrastructure.Persistence.Shared.Contexts;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityUserContext<IdentityUser>(options)
{
    public DbSet<User> DomainUsers => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ProductClassification> ProductClassifications => Set<ProductClassification>();
    public DbSet<CompanyFavorite> CompanyFavorites => Set<CompanyFavorite>();
    public DbSet<ProductFavorite> ProductFavorites => Set<ProductFavorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
