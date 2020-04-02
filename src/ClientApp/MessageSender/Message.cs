using System;

namespace MessageSender
{
    public class Message
    {
        public long Id { get; }
        public string Content { get; }

        public DateTimeOffset CreatedAt { get; }
        public Message(long id, string content, DateTimeOffset createdAt)
        {
            Id = id;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}