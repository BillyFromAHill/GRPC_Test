using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using Persistence;

namespace ClientApp
{
    class Program
    {
        private static IContainer Container { get; set; }

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        static void Main(string[] args)
        {
            InitDi();
            
            Console.WriteLine("Hello World!");
        }

        private static void InitDi()
        {
            var builder = new ContainerBuilder();
            builder.AddMediatR();
            builder.RegisterType<MessagesDbContext>().InstancePerLifetimeScope();
            Container = builder.Build();
        }
    }
}