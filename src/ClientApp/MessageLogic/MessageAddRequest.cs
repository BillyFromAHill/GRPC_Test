using System;
using CSharpFunctionalExtensions;
using MediatR;

namespace MessageLogic
{
    public class MessageAddRequest : IRequest<Result<long, string>>
    {
        public string Message { get; }

        public MessageAddRequest(string message)
        {
            Message = message;
        }
    }
}