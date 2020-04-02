using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using MessageReceiver;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class ServerAppService : MessagesService.MessagesServiceBase
    {
        private readonly ILogger<ServerAppService> _logger;
        private readonly IMessageRepository _messageRepository;

        public ServerAppService(ILogger<ServerAppService> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }

        public override Task<SaveResult> SendMessages(MessageList request, ServerCallContext context)
        {
            return null;
        }
    }
}