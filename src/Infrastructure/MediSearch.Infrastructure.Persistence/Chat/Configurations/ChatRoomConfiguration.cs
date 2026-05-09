using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.Chat.Messages;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Infrastructure.Persistence.Chat.Configurations;

internal sealed class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
{
    public void Configure(EntityTypeBuilder<ChatRoom> builder)
    {
        builder.ToTable("chat_rooms");

        builder
            .Property(cr => cr.Id)
            .HasConversion(new EntityIdValueConverter<ChatRoom>())
            .ValueGeneratedNever();

        builder
            .Property(cr => cr.LastMessageId)
            .HasConversion(new NullableEntityIdValueConverter<Message>());

        builder.OwnsMany(
            cr => cr.Participants,
            j =>
            {
                j.ToTable("chat_room_participants");
                j.Property<int>("id").ValueGeneratedOnAdd();
                j.HasKey("id");
                j.WithOwner().HasForeignKey("chat_room_id");

                j.OwnsOne(
                    p => p.Agent,
                    a =>
                    {
                        a.Property(x => x.AgentId).HasColumnName("participant_id");
                        a.Property(x => x.AgentTypeId).HasColumnName("participant_type_id");
                        a.HasOne<AgentType>()
                            .WithMany()
                            .HasForeignKey(x => x.AgentTypeId)
                            .OnDelete(DeleteBehavior.Restrict);
                    }
                );

                j.Property(p => p.LastCheckedAt).HasColumnName("last_checked_at");
            }
        );

        builder.HasKey(cr => cr.Id);

        builder
            .HasOne<Message>()
            .WithOne()
            .HasForeignKey<ChatRoom>(cr => cr.LastMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
