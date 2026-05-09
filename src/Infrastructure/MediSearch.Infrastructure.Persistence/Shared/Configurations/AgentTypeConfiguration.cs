using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Infrastructure.Persistence.Shared.Configurations;

internal sealed class AgentTypeConfiguration : IEntityTypeConfiguration<AgentType>
{
    public void Configure(EntityTypeBuilder<AgentType> builder)
    {
        builder.ToTable("agent_types");

        builder.HasKey(agentType => agentType.Id);

        builder.Property(agentType => agentType.Name).HasMaxLength(50);

        builder.HasData(AgentType.User, AgentType.Company);
    }
}
