using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageSender;
using Microsoft.EntityFrameworkCore;
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
            await _messagesDbContext.Set<MessageOutboxItem>().AddAsync(new MessageOutboxItem {MessageId = messageId, SentAt = null}, CancellationToken.None);
        }

        public async Task MarkSent(IEnumerable<long> messageId, CancellationToken cancellationToken)
        {
            // Definitely not optimal solution.
            var chunk = await _messagesDbContext.Set<MessageOutboxItem>().Where(mo => messageId.Contains(mo.MessageId)).ToListAsync(cancellationToken);
            foreach (var message in chunk)
            {
                message.SentAt = DateTimeOffset.Now;
            }

            await _messagesDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}