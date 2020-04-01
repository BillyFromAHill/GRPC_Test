using System.Threading;
using System.Threading.Tasks;

namespace MessageSender
{
    public interface IMessageOutboxRepository
    {
        Task AddMessageAsync(long messageId, CancellationToken cancellationToken);
    }
}