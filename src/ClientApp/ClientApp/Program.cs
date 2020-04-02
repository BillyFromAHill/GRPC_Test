using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using MessageLogic;
using MessageSender;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;

namespace ClientApp
{
    class Program
    {
        private static IContainer Container { get; set; }

        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            InitDi();

            MigrateDb();

            var messageReceiver = Container.Resolve<MessageReceiver>();
            var messageSender = Container.Resolve<MessageSender.MessageSender>();

            var finishedTask = await Task.WhenAny(
                messageReceiver.StartReceiving(CancellationTokenSource.Token),
                messageSender.StartSending(CancellationTokenSource.Token));

            if (finishedTask.IsFaulted)
            {
                Console.WriteLine("Looks like something went wrong. See logs for details.");
            }
        }

        private static void MigrateDb()
        {
            var dbContext = Container.Resolve<MessagesDbContext>();
            dbContext.Database.Migrate();
        }

        private static void InitDi()
        {
            var builder = new ContainerBuilder();
            builder.AddMediatR(typeof(MessageAddedEvent).Assembly, typeof(MessageAddedEventHandler).Assembly, typeof(SendQueueRequestHandler).Assembly);
            builder.RegisterType<MessageOutboxRepository>().AsImplementedInterfaces();
            builder.RegisterType<MessageRepository>().AsImplementedInterfaces();
            builder.RegisterType<MessageReceiver>();
            builder.RegisterType<MessageSender.MessageSender>();

            builder.Register(componentContext =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<MessagesDbContext>()
                        .UseSqlite("Data Source=./Messages.db");
                    return optionsBuilder.Options;
                }).As<DbContextOptions>()
                .InstancePerLifetimeScope();
            builder.RegisterType<MessagesDbContext>().InstancePerLifetimeScope();

            Container = builder.Build();
        }
    }
}