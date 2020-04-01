using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using MessageLogic;

namespace ClientApp
{
    public class MessageReceiver
    {
        private readonly IMediator _mediator;

        public MessageReceiver(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task StartReceiving(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
               var messageString = await Console.In.ReadLineAsync();

               var (_, isFailure, _, error) = await _mediator.Send(new MessageAddRequest(messageString), cancellationToken);

               Console.WriteLine(isFailure ? $"Сообщение не принято. {error}" : $"Сообщение принято.");
            }
        }
    }
}