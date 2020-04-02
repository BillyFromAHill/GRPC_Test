﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Persistence;

namespace ClientApp
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ServerDbContext>
    {
        public ServerDbContext CreateDbContext(string[] args)
        {
            return new ServerDbContext(new DbContextOptionsBuilder<ServerDbContext>().UseSqlite("Data Source=./Messages.db").Options);
        }
    }
}