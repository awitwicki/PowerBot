using Microsoft.EntityFrameworkCore;
using PowerBot.Core.Data;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerBot.Core.Managers
{
    public static class StatsManager
    {
        public static PowerBotContext _dbContext = new PowerBotContext();

        public static async Task<List<Stats>> GetStats(DateTime dateFrom, DateTime dateTo)
        {
            var stats = await _dbContext.Stats.AsNoTracking()
                .Where(x => x.DateTime >= dateFrom && x.DateTime < dateTo)
                .ToListAsync();

            return stats;
        }
       
        public static async Task AddStatAction(ActionType actionType)
        {
            await _dbContext.Stats.AddAsync(
                new Stats 
                { 
                    DateTime = DateTime.UtcNow,
                    ActionType = actionType
                });

            await _dbContext.SaveChangesAsync();
        }
    }
}
