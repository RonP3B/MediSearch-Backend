using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.Chat.Messages;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Infrastructure.Persistence.Chat.Configurations;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder
            .Property(m => m.Id)
            .HasConversion(new EntityIdValueConverter<Message>())
            .ValueGeneratedNever();

        builder.Property(m => m.ChatRoomId).HasConversion(new EntityIdValueConverter<ChatRoom>());

        builder
            .Property(m => m.TextContent)
            .HasConversion(new NullableCleanTextValueConverter())
            .HasMaxLength(300);

        builder.OwnsOne(
            m => m.SenderAgent,
            b =>
            {
                b.Property(a => a.AgentId).HasColumnName("sender_id");
                b.Property(a => a.AgentTypeId).HasColumnName("sender_type_id");
                b.HasOne<AgentType>()
                    .WithMany()
                    .HasForeignKey(a => a.AgentTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        );

        builder.OwnsOne(
            m => m.MediaContent,
            b =>
            {
                b.Property(mc => mc.AssetKey)
                    .HasConversion(new AssetKeyValueConverter())
                    .HasMaxLength(500)
                    .HasColumnName("media_content_asset_key");

                b.Property(mc => mc.MediaType)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .HasColumnName("media_content_type");
            }
        );

        builder.HasKey(m => m.Id);
        builder.HasIndex(m => m.ChatRoomId);
        builder.HasIndex(m => new { m.ChatRoomId, m.MessageSentDate });

        builder
            .HasOne<ChatRoom>()
            .WithMany()
            .HasForeignKey(m => m.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
