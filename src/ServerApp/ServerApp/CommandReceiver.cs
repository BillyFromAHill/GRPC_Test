using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Queries;


namespace ServerApp
{
    public class CommandReceiver
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommandReceiver> _logger;

        public CommandReceiver(IMediator mediator, ILogger<CommandReceiver> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task StartReceiving(CancellationToken cancellationToken)
        {
            return Task.Run(async () => { await RunMessageLoop(cancellationToken); }, cancellationToken);
        }

        private async Task RunMessageLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var commandString = await Console.In.ReadLineAsync();

                    var commandArgs = commandString.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (!commandArgs.Any())
                    {
                        continue;
                    }

                    if (!commandArgs[0].Trim().ToLower().Equals("print"))
                    {
                        Console.WriteLine("Unknown command. Enter 'print [offset] [count]' to show records.");
                        continue;
                    }

                    ulong offset = 0;
                    if (commandArgs.Count > 1)
                    {
                        ulong.TryParse(commandArgs[1], out offset);
                    }

                    ulong count = 100;
                    if (commandArgs.Count > 2)
                    {
                        ulong.TryParse(commandArgs[2], out count);
                    }

                    if (count > 1000)
                    {
                        Console.WriteLine($"Count {count} is too large specify lower value");
                        continue;
                    }

                    var messagesPage = await _mediator.Send(new MessagesQuery(offset, count), cancellationToken);
                    foreach (var message in messagesPage.Messages)
                    {
                        Console.WriteLine($"{message.ClientIp} - {message.Content}");
                    }

                    Console.WriteLine(
                        $"Messages from {offset} to {offset + count}. Total records: {messagesPage.TotalCount}. To show more records write 'print [offset] [count]'");
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