using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using JobTimer.Data.Dtos;
using JobTimer.Data.Dtos.Timer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Utils;
using Timer = JobTimer.Data.Dtos.Timer.Timer;

namespace JobTimer.Data.Access.JobTimer
{
    public interface ITimerAccess : IAccess<int, Model.JobTimer.Timer>
    {
        Task<Model.JobTimer.Timer> GetLastAsyncOfToday(string username, List<int> timerTypes, TimeZoneInfo tz = null);
        Task<Model.JobTimer.Timer> GetLastAsyncOfDate(string username, List<int> timerTypes, DateTime date);
        Task<EnterExitRanges> GetRangesOfThisWeek(string username);
        Task<EnterExitRanges> GetRangesOfThisMonth(string username);
        Task<EnterExitRanges> GetRangesOfLunchOfThisWeek(string username);
        Task<EnterExitRanges> GetRangesOfLunchOfThisMonth(string username);
        Task<EnterExitRanges> GetRangesWithLunchOfToday(string username);
        Task<EnterExitRanges> GetRangesWithLunchOfThisWeek(string username);
        Task<EnterExitRanges> GetRangesWithLunchOfThisMonth(string username);

        Task<EnterExit> GetAllWithLunchOfToday(string username);
        Task<EnterExit> GetAllWithLunchOfDate(string username, DateTime date);
        Task<EnterExit> GetAllWithLunchOfWeek(string username);
        Task<EnterExit> GetAllWithLunchOfMonth(string username);
        Task<EnterExit> GetAllOfLunchOfWeek(string username);
        Task<EnterExit> GetAllOfLunchOfMonth(string username);
    }
    public class TimerAccess : AccessWithInt<Model.JobTimer.Timer>, ITimerAccess
    {
        private readonly ITimeBuilder _timeBuilder;

        public TimerAccess(JobTimerDbContext context, ITimeBuilder timeBuilder)
            : base(context)
        {
            _timeBuilder = timeBuilder;
        }

        public async Task<Model.JobTimer.Timer> GetLastAsyncOfToday(string username, List<int> timerTypes, TimeZoneInfo tz = null)
        {
            var now = DateTime.UtcNow;

            if (tz != null)
                now = TimeZoneInfo.ConvertTimeFromUtc(now, tz);

            return await GetLastAsyncOfDate(username, timerTypes, now);
        }

        public async Task<Model.JobTimer.Timer> GetLastAsyncOfDate(string username, List<int> timerTypes, DateTime date)
        {
            return await Set.Where(x =>
                x.UserName == username &&
                timerTypes.Contains(x.TimerTypeId) &&
                DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date))
                .OrderByDescending(x => DbFunctions.TruncateTime(x.Date))
                .ThenByDescending(x => x.Time)
                .FirstOrDefaultAsync();
        }

        #region ranges
        public async Task<EnterExitRanges> GetRangesOfThisWeek(string username)
        {
            var enters = new List<int>() { (int)TimerTypes.Enter };
            var exits = new List<int>() { (int)TimerTypes.Exit };

            var start = _timeBuilder.GetWeekStartDelimiter();
            var end = _timeBuilder.GetWeekEndDelimiter();

            return await GetRangesOf(username, enters, exits, start, end);
        }

        public async Task<EnterExitRanges> GetRangesOfThisMonth(string username)
        {
            var enters = new List<int>() { (int)TimerTypes.Enter };
            var exits = new List<int>() { (int)TimerTypes.Exit };

            var start = _timeBuilder.GetMonthStartDelimiter();
            var end = _timeBuilder.GetMonthEndDelimiter();

            return await GetRangesOf(username, enters, exits, start, end);
        }

        public async Task<EnterExitRanges> GetRangesOfLunchOfThisWeek(string username)
        {
            var enters = new List<int>() { (int)TimerTypes.EnterLunch };
            var exits = new List<int>() { (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetWeekStartDelimiter();
            var end = _timeBuilder.GetWeekEndDelimiter();

            return await GetRangesOf(username, enters, exits, start, end);
        }

        public async Task<EnterExitRanges> GetRangesOfLunchOfThisMonth(string username)
        {
            var enters = new List<int>() { (int)TimerTypes.EnterLunch };
            var exits = new List<int>() { (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetMonthStartDelimiter();
            var end = _timeBuilder.GetMonthEndDelimiter();

            return await GetRangesOf(username, enters, exits, start, end);
        }

        public async Task<EnterExitRanges> GetRangesWithLunchOfToday(string username)
        {
            var enters = new List<int>() { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch };
            var exits = new List<int>() { (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetTodayStartDelimiter();
            var end = _timeBuilder.GetTodayEndDelimiter();

            return await GetRangesOf(username, enters, exits, start, end);
        }

        public async Task<EnterExitRanges> GetRangesWithLunchOfThisWeek(string username)
        {
            var enters = new List<int>() { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch };
            var exits = new List<int>() { (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetWeekStartDelimiter();
            var end = _timeBuilder.GetWeekEndDelimiter();

            return await GetRangesOf(username, enters, exits, start, end);
        }

        public async Task<EnterExitRanges> GetRangesWithLunchOfThisMonth(string username)
        {
            var enters = new List<int>() { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch };
            var exits = new List<int>() { (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetMonthStartDelimiter();
            var end = _timeBuilder.GetMonthEndDelimiter();

            return await GetRangesOf(username, enters, exits, start, end);
        }

        private async Task<EnterExitRanges> GetRangesOf(string username, List<int> enters, List<int> exits, DateTime start, DateTime end)
        {
            var result = new EnterExitRanges
            {
                EnterTimers = await GetRangeEnterTimers(username, enters, start, end),
                ExitTimers = await GetRangeExitTimers(username, exits, start, end)
            };

            return result;
        }

        private async Task<List<Timer>> GetRangeEnterTimers(string username, List<int> enters, DateTime start, DateTime end)
        {
            return await GetRangeEnterTimer(username, enters, start, end).ToListAsync();
        }

        private IQueryable<Timer> GetRangeEnterTimer(string username, List<int> enters, DateTime start, DateTime end)
        {
            return from row in Set
                   where enters.Contains(row.TimerTypeId) &&
                         row.UserName == username &&
                         row.Date >= start &&
                         row.Date <= end
                   orderby DbFunctions.TruncateTime(row.Date), row.Time
                   group row by new { Date = DbFunctions.TruncateTime(row.Date), row.TimerTypeId } into g
                   select new Timer()
                   {
                       TimerType = g.FirstOrDefault().TimerTypeId,
                       Date = g.FirstOrDefault().Date,
                       Time = g.Min(m => m.Time)
                   };
        }

        private async Task<List<Timer>> GetRangeExitTimers(string username, List<int> exits, DateTime start, DateTime end)
        {
            return await GetRangeExitTimer(username, exits, start, end).ToListAsync();
        }

        private IQueryable<Timer> GetRangeExitTimer(string username, List<int> exits, DateTime start, DateTime end)
        {
            return from row in Set
                   where exits.Contains(row.TimerTypeId) &&
                         row.UserName == username &&
                         row.Date >= start &&
                         row.Date <= end
                   orderby DbFunctions.TruncateTime(row.Date), row.Time
                   group row by new { Date = DbFunctions.TruncateTime(row.Date), row.TimerTypeId } into g
                   select new Timer()
                   {
                       TimerType = g.FirstOrDefault().TimerTypeId,
                       Date = g.FirstOrDefault().Date,
                       Time = g.Max(m => m.Time)
                   };
        }
        #endregion

        public async Task<EnterExit> GetAllWithLunchOfToday(string username)
        {
            var types = new List<int>() { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch, (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetTodayStartDelimiter();
            var end = _timeBuilder.GetTodayEndDelimiter();

            return await GetAllOf(username, types, start, end);
        }

        public async Task<EnterExit> GetAllWithLunchOfDate(string username, DateTime date)
        {
            var types = new List<int>() { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch, (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetDateStartDelimiter(date);
            var end = _timeBuilder.GetDateEndDelimiter(date);

            return await GetAllOf(username, types, start, end);
        }

        public async Task<EnterExit> GetAllWithLunchOfWeek(string username)
        {
            var types = new List<int>() { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch, (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetWeekStartDelimiter();
            var end = _timeBuilder.GetWeekEndDelimiter();

            return await GetAllOf(username, types, start, end);
        }

        public async Task<EnterExit> GetAllWithLunchOfMonth(string username)
        {
            var types = new List<int>() { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch, (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetMonthStartDelimiter();
            var end = _timeBuilder.GetMonthEndDelimiter();

            return await GetAllOf(username, types, start, end);
        }

        public async Task<EnterExit> GetAllOfLunchOfWeek(string username)
        {
            var types = new List<int>() { (int)TimerTypes.EnterLunch, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetWeekStartDelimiter();
            var end = _timeBuilder.GetWeekEndDelimiter();

            return await GetAllOf(username, types, start, end);
        }

        public async Task<EnterExit> GetAllOfLunchOfMonth(string username)
        {
            var types = new List<int>() { (int)TimerTypes.EnterLunch, (int)TimerTypes.ExitLunch };

            var start = _timeBuilder.GetMonthStartDelimiter();
            var end = _timeBuilder.GetMonthEndDelimiter();

            return await GetAllOf(username, types, start, end);
        }

        private async Task<EnterExit> GetAllOf(string username, List<int> types, DateTime start, DateTime end)
        {
            var result = new EnterExit();

            var timers = await GetAllEnterTimer(username, types, start, end);

            var splitted = timers.GroupBy(x => x.TimerType).ToDictionary(g => g.Key, g => g.ToList());

            if (splitted.ContainsKey((int)TimerTypes.Enter))
                result.EnterTimers = splitted[(int)TimerTypes.Enter];
            if (splitted.ContainsKey((int)TimerTypes.EnterLunch))
                result.EnterLunchTimers = splitted[(int)TimerTypes.EnterLunch];
            if (splitted.ContainsKey((int)TimerTypes.Exit))
                result.ExitTimers = splitted[(int)TimerTypes.Exit];
            if (splitted.ContainsKey((int)TimerTypes.ExitLunch))
                result.ExitLunchTimers = splitted[(int)TimerTypes.ExitLunch];

            return result;
        }

        private async Task<List<Timer>> GetAllEnterTimer(string username, List<int> types, DateTime start, DateTime end)
        {
            var query = from row in Set
                        where types.Contains(row.TimerTypeId) &&
                              row.UserName == username &&
                              row.Date >= start &&
                              row.Date <= end
                        orderby DbFunctions.TruncateTime(row.Date), row.Time
                        select new Timer()
                        {
                            TimerType = row.TimerTypeId,
                            Date = row.Date,
                            Time = row.Time
                        };

            return await query.ToListAsync();
        }
    }
}
