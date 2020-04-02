using System;
using MediatR;

namespace Queries
{
    public class MessagesQuery : IRequest<MessagePage>
    {
        public ulong Offset { get; }
        public ulong Count { get; }

        public MessagesQuery(ulong offset = 0, ulong count = 1000)
        {
            Offset = offset;
            Count = count;
        }
    }
}