namespace MessageSender
{
    public class Message
    {
        public long Id { get; }
        public string Content { get; }

        public Message(long id, string content)
        {
            Id = id;
            Content = content;
        }
    }
}