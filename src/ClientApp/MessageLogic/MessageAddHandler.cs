﻿using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;

namespace MessageLogic
{
    public class MessageAddHandler : IRequestHandler<MessageAddRequest, Result<long, string>>
    {
        private readonly IMediator _mediator;
        private readonly IMessageRepository _messageRepository;
        private readonly int MessageMaxSize = 3000;

        public MessageAddHandler(IMediator mediator, IMessageRepository messageRepository)
        {
            _mediator = mediator;
            _messageRepository = messageRepository;
        }

        public async Task<Result<long, string>> Handle(MessageAddRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return Result.Failure<long, string>("Message must be not empty");
            }

            if (request.Message.Length > MessageMaxSize)
            {
                return Result.Failure<long, string>($"Message must be no longer than {MessageMaxSize}.");
            }

            // This calls should be done in one opened in mediator transaction. Now it's not safe.
            var messageId = await _messageRepository.SaveMessageAsync(request.Message, cancellationToken);
            await _mediator.Publish(new MessageAddedEvent(messageId), cancellationToken);

            return Result.Ok<long, string>(messageId);
        }
    }
}