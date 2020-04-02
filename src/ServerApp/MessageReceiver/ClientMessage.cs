using System;

namespace MessageReceiver
{
    public class ClientMessage
    {
        public long MessageId { get; }
        public string Content { get; }
        public DateTimeOffset CreatedAt { get; }
        public string IpAddress { get; }

        public ClientMessage(long messageId, string content, DateTimeOffset createdAt, string ipAddress)
        {
            MessageId = messageId;
            Content = content;
            CreatedAt = createdAt;
            IpAddress = ipAddress;
        }
    }
}