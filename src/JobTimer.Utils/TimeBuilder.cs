using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using AutoMapper;
using NodaTime.TimeZones;

namespace JobTimer.Utils
{
    public interface ITimeBuilder
    {
        DateTime GetTodayStartDelimiter();
        DateTime GetTodayEndDelimiter();
        DateTime GetDateStartDelimiter(DateTime date);
        DateTime GetDateEndDelimiter(DateTime date);
        DateTime GetWeekStartDelimiter();
        DateTime GetWeekEndDelimiter();
        DateTime GetMonthStartDelimiter();
        DateTime GetMonthEndDelimiter();
        List<DayOfWeek> GetDayOfWeeks();
        List<DateTime> GetWeek();
        int GetMonths();
        long ToJavascriptTicks(DateTime date, TimeZoneInfo tz = null);
        long ToJavascriptTicks(DateTime date, TimeSpan time, TimeZoneInfo tz = null);
        DateTime UpdateTime(DateTime date, TimeSpan time);
        TimeZoneInfo GetTimeZoneInfo(string olsonTimeZone);
        TimeSpan ConvertTimeSpanToTimeZoneInfo(TimeSpan ts, TimeZoneInfo tz);
        TimeSpan RemoveSeconds(TimeSpan ts);
    }

    public class TimeBuilder : ITimeBuilder
    {
        private readonly ITimeGetter _timeGetter;

        public TimeBuilder(ITimeGetter timeGetter)
        {
            _timeGetter = timeGetter;
        }

        public TimeBuilder()
        {
            throw new NotImplementedException();
        }

        private int GetDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                default:
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                case DayOfWeek.Sunday:
                    return 7;
            }
        }

        public List<DayOfWeek> GetDayOfWeeks()
        {
            return new List<DayOfWeek>()
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };
        }

        public DateTime GetTodayStartDelimiter()
        {
            var now = _timeGetter.Now;
            return GetDateStartDelimiter(now);
        }

        public DateTime GetTodayEndDelimiter()
        {
            var now = _timeGetter.Now;
            return GetDateEndDelimiter(now);
        }

        public DateTime GetDateStartDelimiter(DateTime date)
        {
            date = date.AddMilliseconds(-date.TimeOfDay.TotalMilliseconds);
            return date;
        }

        public DateTime GetDateEndDelimiter(DateTime date)
        {
            date = date.AddHours(24 - date.TimeOfDay.TotalHours).AddSeconds(-1);
            return date;
        }

        public DateTime GetWeekStartDelimiter()
        {
            var now = _timeGetter.Now;
            if (now.DayOfWeek != DayOfWeek.Monday)
            {
                now = now.AddDays(-(GetDayOfWeek(now.DayOfWeek) - 1));
            }
            now = now.AddMilliseconds(-now.TimeOfDay.TotalMilliseconds);
            return now;
        }

        public DateTime GetWeekEndDelimiter()
        {
            var now = _timeGetter.Now;
            if (now.DayOfWeek != DayOfWeek.Sunday)
            {
                now = now.AddDays(7 - GetDayOfWeek(now.DayOfWeek));
            }
            now = now.AddHours(24 - now.TimeOfDay.TotalHours).AddSeconds(-1);
            return now;
        }

        public DateTime GetMonthStartDelimiter()
        {
            var now = _timeGetter.Now;

            return new DateTime(now.Year, now.Month, 1);
        }

        public DateTime GetMonthEndDelimiter()
        {
            var now = _timeGetter.Now;

            var date = new DateTime(now.Year, now.Month, 1);

            date = date.AddMonths(1).AddDays(-1);

            return date;
        }

        public List<DateTime> GetWeek()
        {
            var endDelimiter = GetWeekEndDelimiter();
            var startDelimiter = GetWeekStartDelimiter();

            var days = endDelimiter - startDelimiter;

            var counter = 0;
            var week = new List<DateTime>();
            var currentDate = startDelimiter;
            while (counter < days.TotalDays && counter <= 7)
            {
                week.Add(currentDate);
                counter++;
                currentDate = currentDate.AddDays(1);
            }
            return week;
        }

        public int GetMonths()
        {
            return GetMonthEndDelimiter().Day;
        }

        public long ToJavascriptTicks(DateTime date, TimeZoneInfo tz = null)
        {
            if (tz != null)
                date = TimeZoneInfo.ConvertTimeFromUtc(date, tz);

            var ticks = (date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            ticks = ticks * 1000;
            return (long)ticks;
        }

        public long ToJavascriptTicks(DateTime date, TimeSpan time, TimeZoneInfo tz = null)
        {
            date = UpdateTime(date, time);

            if (tz != null)
                date = TimeZoneInfo.ConvertTimeFromUtc(date, tz);

            var ticks = (date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            ticks = ticks * 1000;
            return (long)ticks;
        }

        public DateTime UpdateTime(DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }

        public TimeZoneInfo GetTimeZoneInfo(string olsonTimeZone)
        {
            var map = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones.FirstOrDefault(x => x.TzdbIds.Any(z => z.Equals(olsonTimeZone, StringComparison.OrdinalIgnoreCase)));
            return map == null ? TimeZoneInfo.Utc : TimeZoneInfo.FindSystemTimeZoneById(map.WindowsId);
        }

        public TimeSpan ConvertTimeSpanToTimeZoneInfo(TimeSpan ts, TimeZoneInfo tz)
        {
            var updated = UpdateTime(DateTime.UtcNow, ts);

            var converted = TimeZoneInfo.ConvertTimeFromUtc(updated, tz);

            return converted.TimeOfDay;
        }

        public TimeSpan RemoveSeconds(TimeSpan ts)
        {
            return new TimeSpan(ts.Ticks - (ts.Ticks % TimeSpan.TicksPerMinute));
        }
    }
}
