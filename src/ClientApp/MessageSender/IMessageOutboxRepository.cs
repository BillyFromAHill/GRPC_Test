using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MessageSender
{
    public interface IMessageOutboxRepository
    {
        Task AddMessageAsync(long messageId, CancellationToken cancellationToken);

        Task MarkSent(IEnumerable<long> messageId, CancellationToken cancellationToken);
    }
}