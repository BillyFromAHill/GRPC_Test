using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MessageSender;
using Microsoft.EntityFrameworkCore;
using Persistence.Models;
using Message = MessageSender.Message;

namespace Persistence
{
    public class SendQueueRequestHandler : IRequestHandler<SendQueueRequest, IEnumerable<Message>>
    {
        private readonly MessagesDbContext _messagesDbContext;

        public SendQueueRequestHandler(MessagesDbContext messagesDbContext)
        {
            _messagesDbContext = messagesDbContext;
        }

        public async Task<IEnumerable<Message>> Handle(SendQueueRequest request, CancellationToken cancellationToken)
        {
            return (await _messagesDbContext.Set<MessageOutboxItem>()
                    .Include(mo => mo.Message)
                    .Where(mo => !mo.SentAt.HasValue)
                    .Take((int) request.ChunkSize)
                    .ToListAsync(cancellationToken))
                .Select(mo => new Message(mo.MessageId, mo.Message.Content));
        }
    }
}