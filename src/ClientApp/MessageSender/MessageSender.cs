using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using MessageLogic;
using Microsoft.Extensions.Logging;
using Services;

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
                    using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                    var client = new MessagesService.MessagesServiceClient(channel);

                    var messagesChunk = (await _mediator.Send(new SendQueueRequest(_chunkSize), cancellationToken)).ToList();

                    if (!messagesChunk.Any())
                    {
                        continue;
                    }

                    var messageList = new MessageList();
                    messageList.ClientId = "123";
                    messageList.Messages.AddRange(messagesChunk.Select(m => new MessageDto
                        {Content = m.Content, MessageId = m.Id, CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(m.CreatedAt)}));

                    await client.SendMessagesAsync(messageList);

                    await _messageOutboxRepository.MarkSent(messagesChunk.Select(m => m.Id), cancellationToken);
                    await Task.Delay((int) _delayMs, cancellationToken);

                    if (!messagesChunk.Any())
                    {
                        await Task.Delay((int) _delayMs, cancellationToken);
                    }

                }
                catch (RpcException ex)
                {
                    _logger.Log(LogLevel.Error, ex.ToString());
                    await Task.Delay((int) _delayMs, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, $"{e.Message} {e.StackTrace}");
                    throw;
                }
            }

            _logger.Log(LogLevel.Information, "Message send has stopped.");
        }
    }
}