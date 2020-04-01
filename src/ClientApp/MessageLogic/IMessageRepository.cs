using System.Threading;
using System.Threading.Tasks;

namespace MessageLogic
{
    public interface IMessageRepository
    {
        Task<long> SaveMessageAsync(string message, CancellationToken cancellationToken);
    }
}