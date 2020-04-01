using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence
{
    public class MessagesDbContext : DbContext
    {
        public MessagesDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Message).Assembly);
        }
    }
}