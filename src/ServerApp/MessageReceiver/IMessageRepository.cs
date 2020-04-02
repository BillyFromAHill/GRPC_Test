using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiver
{
    public interface IMessageRepository
    {
        Task SaveMessages(MessageList messageList, CancellationToken cancellationToken);
    }
}