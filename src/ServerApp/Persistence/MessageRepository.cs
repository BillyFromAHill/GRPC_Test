using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageReceiver;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ServerDbContext _dbContext;

        public MessageRepository(ServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveMessages(MessageList messageList, CancellationToken cancellationToken)
        {
            var messageIds = messageList.Messages.Select(m => m.MessageId).ToList();

            var existingMessages = await _dbContext.Set<ClientMessage>()
                .Where(cm => cm.ClientId == messageList.ClientId)
                .Where(cm => messageIds.Contains(cm.MessageId))
                .ToListAsync(cancellationToken);

            var messageToAdd = messageList.Messages.Where(m => !existingMessages.Select(em => em.MessageId).ToList().Contains(m.MessageId));

            // CLient id should be moved to separate table.
            await _dbContext.Set<ClientMessage>().AddRangeAsync(messageToAdd.Select(
                m => new ClientMessage
                {
                    ClientId = messageList.ClientId, Content = m.Content, CreatedAt = m.CreatedAt, IpAddress = m.IpAddress, MessageId = m.MessageId,
                    ReceivedAt = DateTimeOffset.Now
                }), cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}