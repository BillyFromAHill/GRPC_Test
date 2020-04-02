using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MessageLogic;

namespace MessageSender
{
    public class MessageSender
    {
        private readonly uint _chunkSize = 1000;
        private readonly uint _delayMs = 5000;
        private readonly IMediator _mediator;
        private readonly IMessageOutboxRepository _messageOutboxRepository;

        public MessageSender(IMediator mediator, IMessageOutboxRepository messageOutboxRepository)
        {
            _mediator = mediator;
            _messageOutboxRepository = messageOutboxRepository;
        }

        public Task StartSending(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () => { await RunMessageLoop(cancellationToken); }, TaskCreationOptions.LongRunning);
        }

        private async Task RunMessageLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var messagesChunk = (await _mediator.Send(new SendQueueRequest(_chunkSize), cancellationToken)).ToList();
                foreach (var message in messagesChunk)
                {
                    //TODO: send here
                }

                //await _messageOutboxRepository.MarkSent(messagesChunk.Select(m => m.Id), cancellationToken);

                if (!messagesChunk.Any())
                {
                    await Task.Delay((int)_delayMs, cancellationToken);
                }
            }
        }
    }
}