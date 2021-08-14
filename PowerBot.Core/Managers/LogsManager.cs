using Microsoft.EntityFrameworkCore;
using PowerBot.Core.Data;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PowerBot.Core.Managers
{
    public static class LogsManager
    {
        public static PowerBotContext _dbContext = new PowerBotContext();

        public static async Task<List<Log>> GetLogs()
        {
            var logs = await _dbContext.Logs
                .AsNoTracking()
                .OrderByDescending(x => x.DateTime)
                .ToListAsync();

            return logs;
        }
       
        public static async Task CreateLog(string message, LogLevel logLevel)
        {
            Console.WriteLine(message);

            await _dbContext.Logs.AddAsync(new Log
            {
                DateTime = DateTime.Now,
                MessageText = message,
                LogLevel = logLevel
            });

            await _dbContext.SaveChangesAsync();
        }

        public static void CreateLogSync(string message, LogLevel logLevel)
        {
            CreateLog(message, logLevel).GetAwaiter().GetResult();
        }
    }
}
