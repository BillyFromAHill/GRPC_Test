using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
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
        private readonly SenderConfiguration _configuration;

        public MessageSender(ILogger<MessageSender> logger, IMediator mediator, IMessageOutboxRepository messageOutboxRepository,
            SenderConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _messageOutboxRepository = messageOutboxRepository;
            _configuration = configuration;
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

                    if (!messagesChunk.Any())
                    {
                        await Task.Delay((int) _delayMs, cancellationToken);
                        continue;
                    }

                    using var channel = GrpcChannel.ForAddress(_configuration.BaseUrl);
                    var client = new MessagesService.MessagesServiceClient(channel);

                    var messageList = new MessageList();
                    messageList.ClientId = _configuration.ClientId;
                    messageList.Messages.AddRange(messagesChunk.Select(m => new MessageDto
                        {Content = m.Content, MessageId = m.Id, CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(m.CreatedAt)}));

                    var result = await client.SendMessagesAsync(messageList);
                    foreach (var message in result.Messages)
                    {
                        _logger.Log(LogLevel.Warning, $"Looks like something wrong with message with id '{message.MessageId}'. It was skipped by server.");
                    }

                    await _messageOutboxRepository.MarkSent(messagesChunk.Select(m => m.Id), cancellationToken);
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

            _logger.Log(LogLevel.Information, "Message sending has stopped.");
        }
    }
}