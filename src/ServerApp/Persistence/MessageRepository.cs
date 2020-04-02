using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageReceiver;

namespace Persistence
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ServerDbContext _dbContext;

        public MessageRepository(ServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveMessages(IEnumerable<MessageReceiver.ClientMessage> messages, CancellationToken cancellationToken)
        {
            await _dbContext.AddRangeAsync(messages.Select(
                m => new ClientMessage
                {
                    ClientId = m.ClientId, Content = m.Content, CreatedAt = m.CreatedAt, IpAddress = m.IpAddress, MessageId = m.MessageId,
                    ReceivedAt = DateTimeOffset.Now
                }), cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}