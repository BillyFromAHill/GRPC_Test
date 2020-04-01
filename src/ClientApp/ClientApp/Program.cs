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

        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            InitDi();

            MigrateDb();

            var messageReceiver = Container.Resolve<MessageReceiver>();

            await Task.WhenAny(messageReceiver.StartReceiving(_cancellationTokenSource.Token));
        }

        private static void MigrateDb()
        {
            var dbContext = Container.Resolve<MessagesDbContext>();
            dbContext.Database.Migrate();
        }

        private static void InitDi()
        {
            var builder = new ContainerBuilder();
            builder.AddMediatR(typeof(MessageAddedEvent).Assembly, typeof(MessageAddedEventHandler).Assembly);
            builder.RegisterType<MessageOutboxRepository>().AsImplementedInterfaces();
            builder.RegisterType<MessageRepository>().AsImplementedInterfaces();
            builder.RegisterType<MessageReceiver>();
            
            
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