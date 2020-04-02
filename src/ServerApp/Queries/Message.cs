namespace Queries
{
    public class Message
    {
        public string Content { get; }
        public string ClientIp { get; }

        public Message(string content, string clientIp)
        {
            Content = content;
            ClientIp = clientIp;
        }
    }
}