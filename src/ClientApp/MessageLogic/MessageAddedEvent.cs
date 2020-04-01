using MediatR;

namespace MessageLogic
{
    public class MessageAddedEvent : INotification
    {
        public long Id { get; }

        public MessageAddedEvent(long id)
        {
            Id = id;
        }
    }
}