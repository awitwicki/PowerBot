using Microsoft.EntityFrameworkCore;
using PowerBot.Core.Data;
using PowerBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;  
using System.Dynamic;
using System.Threading.Tasks;
using PowerBot.Core.Extensions;

namespace PowerBot.Core.Managers
{
    public static class StatsManager
    {
        public static PowerBotContext _dbContext = new PowerBotContext();

        public static async Task<List<Stats>> GetStats(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var statsQuery = _dbContext.Stats
                .AsNoTracking();

            if (dateFrom.HasValue)
                statsQuery = statsQuery.Where(x => x.DateTime >= dateFrom);

            if (dateTo.HasValue)
                statsQuery = statsQuery.Where(x => x.DateTime < dateTo);

            var stats = await statsQuery.ToListAsync();

            return stats;
        }

        public static async Task<DataItem[]> GetAggregatedStats(ActionType actionType, DateTime? dateFrom = null, DateTime? dateTo = null, TimeRangeAggregationTypes truncateSize = TimeRangeAggregationTypes.Hourly)
        {
            var statsQuery = _dbContext.Stats
                .AsNoTracking()
                .Where(x => x.ActionType == actionType);

            if (dateFrom.HasValue)
                statsQuery = statsQuery.Where(x => x.DateTime >= dateFrom);

            if (dateTo.HasValue)
                statsQuery = statsQuery.Where(x => x.DateTime < dateTo);

            var stats = await statsQuery.ToListAsync();

            var aggregatedStats = stats
                .GroupBy(x => x.DateTime.TruncatedBy(truncateSize))
                .Select(x =>
                    new DataItem
                    {
                        Date = x.First().DateTime,
                        Count = x.Count()
                    })
                .ToArray();

            return aggregatedStats;
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

        public static List<Stats> TimeRangeAggregation(this List<Stats> stats)
        {
            return stats;
        }
    }

    public enum TimeRangeAggregationTypes
    {
        Hourly,
        Daily
    }

    public class DataItem
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
