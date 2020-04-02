using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using MessageLogic;
using Microsoft.Extensions.Logging;

namespace ClientApp
{
    public class MessageReceiver
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MessageReceiver> _logger;

        public MessageReceiver(IMediator mediator, ILogger<MessageReceiver> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task StartReceiving(CancellationToken cancellationToken)
        {
            return Task.Run(async () => {  await RunMessageLoop(cancellationToken); }, cancellationToken);
        }

        private async Task RunMessageLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var messageString = await Console.In.ReadLineAsync();

                    var (_, isFailure, _, error) = await _mediator.Send(new MessageAddRequest(messageString), cancellationToken);

                    Console.WriteLine(isFailure ? $"Message is not received. {error}" : $"Message received.");
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

            _logger.Log(LogLevel.Information, "Message receiving has stopped.");
        }
    }
}