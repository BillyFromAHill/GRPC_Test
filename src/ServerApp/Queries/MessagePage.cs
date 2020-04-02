using System.Collections;
using System.Collections.Generic;

namespace Queries
{
    public class MessagePage
    {
        public IEnumerable<Message> Messages { get; }
        public ulong Offset { get; }
        public ulong TotalCount { get; }

        public MessagePage(IEnumerable<Message> messages, ulong offset, ulong totalCount)
        {
            Messages = messages;
            Offset = offset;
            TotalCount = totalCount;
        }
    }
}