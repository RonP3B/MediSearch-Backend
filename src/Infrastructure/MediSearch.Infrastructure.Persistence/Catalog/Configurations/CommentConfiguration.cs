using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Infrastructure.Persistence.Catalog.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");

        builder
            .Property(c => c.Id)
            .HasConversion(new EntityIdValueConverter<Comment>())
            .ValueGeneratedNever();

        builder.Property(c => c.UserId).HasConversion(new EntityIdValueConverter<User>());

        builder.Property(c => c.ProductId).HasConversion(new EntityIdValueConverter<Product>());

        builder
            .Property(c => c.Content)
            .HasConversion(new CleanTextValueConverter())
            .HasMaxLength(300);

        builder
            .Property(c => c.ParentCommentId)
            .HasConversion(new NullableEntityIdValueConverter<Comment>());

        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.ProductId);
        builder.HasIndex(c => new { c.ProductId, c.CreatedAt });
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.ParentCommentId);
        builder.HasIndex(c => new { c.ParentCommentId, c.CreatedAt });

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Comment>()
            .WithMany()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
