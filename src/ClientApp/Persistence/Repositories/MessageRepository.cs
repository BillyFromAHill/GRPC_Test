using System;
using System.Threading;
using System.Threading.Tasks;
using MessageLogic;
using Persistence.Models;

namespace Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessagesDbContext _messagesDbContext;

        public MessageRepository(MessagesDbContext messagesDbContext)
        {
            _messagesDbContext = messagesDbContext;
        }

        public async Task<long> SaveMessageAsync(string message, CancellationToken cancellationToken)
        {
            var newMessage = new Message {Content = message, CreatedAt = DateTimeOffset.Now};
            await _messagesDbContext.Set<Message>().AddAsync(newMessage, cancellationToken);
            await _messagesDbContext.SaveChangesAsync(cancellationToken);

            return newMessage.Id;
        }
    }
}