using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerBot.Core.Data
{
    public class PowerBotContext : DbContext
    {
        public DbSet<PowerbotUser> Users { get; set; }
        public DbSet<PowerBotChat> Chats { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<Log> Logs { get; set; }

        public PowerBotContext()
        {
            Database.Migrate();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=powerBot.db");
    }
    public class PowerBotContextFactory : IDesignTimeDbContextFactory<PowerBotContext>
    {
        public PowerBotContext CreateDbContext(string[] args)
        {
            return new PowerBotContext();
        }
    }
}
