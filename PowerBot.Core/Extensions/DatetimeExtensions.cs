using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerBot.Core.Managers;

namespace PowerBot.Core.Extensions
{
    public static class DatetimeExtensions
    {
        public static DateTime TruncatedBy(this DateTime date, TimeRangeAggregationTypes truncateSize)
        {
            switch(truncateSize)
            {
                case TimeRangeAggregationTypes.Hourly:
                    return date.TruncatedHourly();

                case TimeRangeAggregationTypes.Daily:
                default:
                    return date.TruncatedDaily();
            }
        }
        public static DateTime TruncatedHourly(this DateTime date)
            => new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);

        public static DateTime TruncatedDaily(this DateTime date)
            => date.Date;
    }
}
