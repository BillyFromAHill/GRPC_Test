using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MessageLogic;
using Microsoft.Extensions.Logging;

namespace MessageSender
{
    public class MessageSender
    {
        private readonly uint _chunkSize = 1000;
        private readonly uint _delayMs = 5000;
        private readonly ILogger<MessageSender> _logger;
        private readonly IMediator _mediator;
        private readonly IMessageOutboxRepository _messageOutboxRepository;

        public MessageSender(ILogger<MessageSender> logger, IMediator mediator, IMessageOutboxRepository messageOutboxRepository)
        {
            _logger = logger;
            _mediator = mediator;
            _messageOutboxRepository = messageOutboxRepository;
        }

        public Task StartSending(CancellationToken cancellationToken)
        {
            return Task.Run(async () => { await RunMessageLoop(cancellationToken); }, cancellationToken);
        }

        private async Task RunMessageLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var messagesChunk = (await _mediator.Send(new SendQueueRequest(_chunkSize), cancellationToken)).ToList();
                    foreach (var message in messagesChunk)
                    {
                        //TODO: send here
                    }

                    //await _messageOutboxRepository.MarkSent(messagesChunk.Select(m => m.Id), cancellationToken);

                    if (!messagesChunk.Any())
                    {
                        await Task.Delay((int) _delayMs, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, $"{e.Message} {e.StackTrace}");
                    throw ;
                }
            }

            _logger.Log(LogLevel.Information, "Message send has stopped.");
        }
    }
}