namespace MediSearch.Core.Domain.Chat.Messages;

public interface IMessageRepository : IRepository
{
    void Add(Message message);
}
