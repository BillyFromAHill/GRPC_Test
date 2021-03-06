﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using GrpcService;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Persistence;
using Queries;
using LogLevel = NLog.LogLevel;

namespace ServerApp
{
    class Program
    {
        private static IContainer Container { get; set; }

        private static IHost AppHost { get; set; }

        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            var logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
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

            await using var commandReceiverScope = Container.BeginLifetimeScope();
            var commandReceiver = commandReceiverScope.Resolve<CommandReceiver>();

            var finishedTask = await Task.WhenAny(commandReceiver.StartReceiving(CancellationTokenSource.Token),
                AppHost.RunAsync(CancellationTokenSource.Token));
            if (finishedTask.IsFaulted)
            {
                Console.WriteLine("Looks like something went wrong. See logs for details.");
            }
        }

        private static void MigrateDb()
        {
            var dbContext = Container.Resolve<ServerDbContext>();
            dbContext.Database.Migrate();
        }

        private static void InitDi(IConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.AddMediatR(typeof(MessageQueryHandler).Assembly, typeof(MessagesQuery).Assembly, typeof(ClientMessage).Assembly);

            PopulateAutofacBuilder(builder, config);

            AppHost = Host.CreateDefaultBuilder()
                .ConfigureLogging(PopulateLogging)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(b => PopulateAutofacBuilder(b, config)))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseConfiguration(config).UseStartup<Startup>(); }).Build();

            Container = builder.Build();
        }

        private static void PopulateLogging(ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddNLog("nlog.config");
        }
        
        private static void PopulateAutofacBuilder(ContainerBuilder builder, IConfiguration config)
        {
            builder.Register(componentContext =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ServerDbContext>()
                        .UseSqlite(config.GetConnectionString("default"));
                    return optionsBuilder.Options;
                }).As<DbContextOptions>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommandReceiver>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(b => b.AddNLog("nlog.config"));

            builder.Populate(serviceCollection);
            builder.RegisterType<ServerDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<MessageRepository>().AsImplementedInterfaces();
        }
    }
}