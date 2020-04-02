using System.Collections.Generic;
using MediatR;

namespace MessageSender
{
    public class SendQueueRequest : IRequest<IEnumerable<Message>>
    {
        public uint ChunkSize { get; }

        public SendQueueRequest(uint chunkSize)
        {
            ChunkSize = chunkSize;
        }
    }
}