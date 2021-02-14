using Microsoft.EntityFrameworkCore;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerBot.Core.Data
{
    public class PowerBotContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public PowerBotContext()
        {
            Database.Migrate();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=powerBot.db");
    }
}
