using System;

namespace MessageReceiver
{
    public class ClientMessage
    {
        public string ClientId { get; }
        public long MessageId { get; }
        public string Content { get; }
        public DateTimeOffset CreatedAt { get; }
        public string IpAddress { get; }

        public ClientMessage(string clientId, long messageId, string content, DateTimeOffset createdAt, string ipAddress)
        {
            ClientId = clientId;
            MessageId = messageId;
            Content = content;
            CreatedAt = createdAt;
            IpAddress = ipAddress;
        }
    }
}