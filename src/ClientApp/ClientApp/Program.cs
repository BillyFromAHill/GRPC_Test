using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;
using MessageLogic;
using MessageSender;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Extensions.Logging;
using Persistence;
using Persistence.Repositories;
using LogLevel = NLog.LogLevel;

namespace ClientApp
{
    class Program
    {
        private static IContainer Container { get; set; }

        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();
                
                InitDi(config);

                MigrateDb();
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Fatal, $"Can't initialize base services. {e.Message} {e.StackTrace}");
                throw;
            }

            await using var receiverScope = Container.BeginLifetimeScope();
            var messageReceiver = receiverScope.Resolve<MessageReceiver>();

            await using var senderScope = Container.BeginLifetimeScope();
            var messageSender = senderScope.Resolve<MessageSender.MessageSender>();

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

        private static void InitDi(IConfigurationRoot config)
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
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(b => b.AddNLog("nlog.config"));

            builder.Populate(serviceCollection);
            builder.RegisterType<MessagesDbContext>().InstancePerLifetimeScope();

            Container = builder.Build();
        }
    }
}