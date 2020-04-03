using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace ClientApp
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MessagesDbContext>
    {
        public MessagesDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            return new MessagesDbContext(new DbContextOptionsBuilder<MessagesDbContext>().UseSqlite(config.GetConnectionString("default")).Options);
        }
    }
}