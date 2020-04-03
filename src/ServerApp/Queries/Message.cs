using System;

namespace Queries
{
    public class Message
    {
        public string Content { get; }
        public string ClientIp { get; }
        public DateTimeOffset CreatedAt { get; }

        public Message(string content, string clientIp, DateTimeOffset createdAt)
        {
            Content = content;
            ClientIp = clientIp;
            CreatedAt = createdAt;
        }
    }
}