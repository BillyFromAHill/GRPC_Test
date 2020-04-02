using System.Collections.Generic;

namespace MessageReceiver
{
    public class MessageList
    {
        public IEnumerable<ClientMessage> Messages { get; }
        public string ClientId { get; }

        public MessageList(IEnumerable<ClientMessage> messages, string clientId)
        {
            Messages = messages;
            ClientId = clientId;
        }
    }
}