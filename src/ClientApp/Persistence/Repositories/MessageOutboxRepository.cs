using System.Threading;
using System.Threading.Tasks;
using MessageSender;
using Persistence.Models;

namespace Persistence.Repositories
{
    public class MessageOutboxRepository : IMessageOutboxRepository
    {
        private readonly MessagesDbContext _messagesDbContext;

        public MessageOutboxRepository(MessagesDbContext messagesDbContext)
        {
            _messagesDbContext = messagesDbContext;
        }

        public async Task AddMessageAsync(long messageId, CancellationToken cancellationToken)
        {
            await _messagesDbContext.Set<MessageOutbox>().AddAsync(new MessageOutbox {MessageId = messageId, SentAt = null}, CancellationToken.None);
        }
    }
}