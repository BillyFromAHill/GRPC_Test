using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Queries;

namespace Persistence
{
    public class MessageQueryHandler : IRequestHandler<MessagesQuery, MessagePage>
    {
        private readonly ServerDbContext _dbContext;

        public MessageQueryHandler(ServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MessagePage> Handle(MessagesQuery request, CancellationToken cancellationToken)
        {
            var count = await _dbContext.Set<ClientMessage>().CountAsync(cancellationToken);

            var messages = await _dbContext.Set<ClientMessage>().Skip((int) request.Offset).Take((int) request.Count).ToListAsync(cancellationToken);
            return new MessagePage(messages.Select(m => new Message(m.Content, m.IpAddress, m.CreatedAt)), request.Offset, (ulong) count);
        }
    }
}