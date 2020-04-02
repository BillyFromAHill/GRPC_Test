using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiver
{
    public interface IMessageRepository
    {
        Task SaveMessages(IEnumerable<ClientMessage> message, CancellationToken cancellationToken);
    }
}