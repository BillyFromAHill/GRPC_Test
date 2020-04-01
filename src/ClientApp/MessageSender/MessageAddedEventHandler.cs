using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MessageLogic;

namespace MessageSender
{
    public class MessageAddedEventHandler : INotificationHandler<MessageAddedEvent>
    {
        private readonly IMessageOutboxRepository _messageOutboxRepository;

        public MessageAddedEventHandler(IMessageOutboxRepository messageOutboxRepository)
        {
            _messageOutboxRepository = messageOutboxRepository;
        }

        public async Task Handle(MessageAddedEvent notification, CancellationToken cancellationToken)
        {
            await _messageOutboxRepository.AddMessageAsync(notification.Id, cancellationToken);
        }
    }
}