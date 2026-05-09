using MediSearch.Core.Domain.Chat.Messages;

namespace MediSearch.Infrastructure.Persistence.Chat.Repositories;

internal sealed class MessageRepository(AppDbContext dbContext) : IMessageRepository
{
    public void Add(Message message)
    {
        dbContext.Messages.Add(message);
    }
}
