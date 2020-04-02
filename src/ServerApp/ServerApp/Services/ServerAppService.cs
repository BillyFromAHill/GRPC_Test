using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using MessageReceiver;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class ServerAppService : MessagesService.MessagesServiceBase
    {
        private readonly ILogger<ServerAppService> _logger;
        private readonly IMessageRepository _messageRepository;

        private readonly uint MaxMessageLength = 3000;

        public ServerAppService(ILogger<ServerAppService> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }

        public override async Task<SkippedMessages> SendMessages(MessageList request, ServerCallContext context)
        {
            var skippedMessages = new SkippedMessages();

            var longMessages = request.Messages.Where(m => m.Content.Length > MaxMessageLength).ToList();
            skippedMessages.Messages.AddRange(longMessages);

            var messagesToSave = request.Messages.Where(m => !longMessages.Contains(m));

            await _messageRepository.SaveMessages(
                new MessageReceiver.MessageList(
                    messagesToSave.Select(mts => new ClientMessage(mts.MessageId, mts.Content, mts.CreatedAt.ToDateTimeOffset(), context.Peer)).ToList(),
                    request.ClientId), context.CancellationToken);

            return skippedMessages;
        }
    }
}