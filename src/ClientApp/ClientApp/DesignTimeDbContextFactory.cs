using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Persistence;

namespace ClientApp
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MessagesDbContext>
    {
        public MessagesDbContext CreateDbContext(string[] args)
        {
            return new MessagesDbContext(new DbContextOptionsBuilder<MessagesDbContext>().UseSqlite("Data Source=./Messages.db").Options);
        }
    }
}