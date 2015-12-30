using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization.Configuration;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Dtos.Timer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Utils;
using JobTimer.WebApplication.Logic.Model.TimerCalculator;

namespace JobTimer.WebApplication.Logic
{
    public interface ITimerCalculator
    {
        Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfThisWeek(string userName);
        Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfThisMonth(string userName);
        Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfLunchOfThisWeek(string userName);
        Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfLunchOfThisMonth(string userName);
        Task<Estimation> CalculateEstimation(string userName);
        Task<TimeSpan> CalculateOvertime(string userName, DateTime date);
    }

    public class TimerCalculator : ITimerCalculator
    {
        private readonly ITimerAccess _timerAccess;
        private readonly ITimeBuilder _timeBuilder;

        public TimerCalculator(ITimerAccess timerAccess, ITimeBuilder timeBuilder)
        {
            _timerAccess = timerAccess;
            _timeBuilder = timeBuilder;
        }

        public async Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfThisWeek(string userName)
        {
            var totals = await _timerAccess.GetAllWithLunchOfWeek(userName);

            return CalculateTotals(totals);
        }

        public async Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfThisMonth(string userName)
        {
            var totals = await _timerAccess.GetAllWithLunchOfMonth(userName);

            return CalculateTotals(totals);
        }

        public async Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfLunchOfThisWeek(string userName)
        {
            var totals = await _timerAccess.GetAllOfLunchOfWeek(userName);

            return CalculateLunchTotals(totals);
        }

        public async Task<Dictionary<DateTime, TimeSpan>> CalculateTotalsOfLunchOfThisMonth(string userName)
        {
            var totals = await _timerAccess.GetAllOfLunchOfMonth(userName);

            return CalculateLunchTotals(totals);
        }

        public async Task<Estimation> CalculateEstimation(string userName)
        {
            var result = new Estimation();

            var todayEnterExit = await _timerAccess.GetAllWithLunchOfToday(userName);

            if (todayEnterExit.EnterTimers.Count == 0 || todayEnterExit.EnterTimers.Count == todayEnterExit.ExitTimers.Count)
                return result;

            var workedTimes = GetSumOfTime(todayEnterExit.EnterTimers, todayEnterExit.ExitTimers);
            var lunchTimes = GetSumOfTime(todayEnterExit.EnterLunchTimers, todayEnterExit.ExitLunchTimers);

            if (workedTimes.Keys.Count > 1 || lunchTimes.Keys.Count > 1)
                throw new Exception("More than one day calculated");

            var workedTime = workedTimes.Values.FirstOrDefault();
            var lunchTime = lunchTimes.Values.FirstOrDefault();

            var lastEnter = todayEnterExit.EnterTimers.LastOrDefault();
            var remainingWorkHours = new TimeSpan(8, 24, 0).Subtract(workedTime).Add(lunchTime);

            var lastEnterWithTime = _timeBuilder.UpdateTime(lastEnter.Date, lastEnter.Time);

            var exitTime = lastEnterWithTime.Add(remainingWorkHours);

            result.HasEstimatedExitTime = true;
            result.EstimatedExitTime = exitTime;

            return result;
        }

        public async Task<TimeSpan> CalculateOvertime(string userName, DateTime date)
        {
            var enterExit = await _timerAccess.GetAllWithLunchOfDate(userName, date);

            if (enterExit.EnterTimers.Count == 0)
                return new TimeSpan();

            var workedTimes = GetSumOfTime(enterExit.EnterTimers, enterExit.ExitTimers);
            var lunchTimes = GetSumOfTime(enterExit.EnterLunchTimers, enterExit.ExitLunchTimers);

            if (workedTimes.Keys.Count > 1 || lunchTimes.Keys.Count > 1)
                throw new Exception("More than one day calculated");

            var workedTime = workedTimes.Values.FirstOrDefault();
            var lunchTime = lunchTimes.Values.FirstOrDefault();

            return workedTime.Subtract(lunchTime).Subtract(new TimeSpan(8, 24, 0));
        }

        private Dictionary<DateTime, TimeSpan> GetSumOfTime(List<Data.Dtos.Timer.Timer> enterTimers, List<Data.Dtos.Timer.Timer> exitTimers)
        {
            var workedTimes = new Dictionary<DateTime, TimeSpan>();

            var enterTimersByDate = enterTimers.GroupBy(x => x.Date.Date).ToDictionary(x => x.Key, x => x.ToList());
            var exitTimersByDate = exitTimers.GroupBy(x => x.Date.Date).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var enterByDate in enterTimersByDate)
            {
                if (!exitTimersByDate.ContainsKey(enterByDate.Key))
                    continue;

                var exitByDate = exitTimersByDate[enterByDate.Key];

                for (int i = 0; i < enterByDate.Value.Count; i++)
                {
                    if (i + 1 > exitByDate.Count)
                        break;

                    var enter = _timeBuilder.UpdateTime(enterByDate.Value[i].Date, enterByDate.Value[i].Time);
                    var exit = _timeBuilder.UpdateTime(exitByDate[i].Date, exitByDate[i].Time);
                    var diff = exit - enter;

                    if (!workedTimes.ContainsKey(enterByDate.Key))
                        workedTimes.Add(enterByDate.Key, diff);
                    else
                        workedTimes[enterByDate.Key] += diff;
                }

            }

            return workedTimes;
        }

        private Dictionary<DateTime, TimeSpan> CalculateTotals(EnterExit weekTotals)
        {
            var result = new Dictionary<DateTime, TimeSpan>();

            var workedTime = GetSumOfTime(weekTotals.EnterTimers, weekTotals.ExitTimers);
            var lunchTime = GetSumOfTime(weekTotals.EnterLunchTimers, weekTotals.ExitLunchTimers);

            foreach (var dateKvp in workedTime)
            {
                var totalHours = dateKvp.Value;
                if (lunchTime.ContainsKey(dateKvp.Key))
                    totalHours -= lunchTime[dateKvp.Key];

                result[dateKvp.Key] = totalHours;
            }

            return result;
        }

        private Dictionary<DateTime, TimeSpan> CalculateLunchTotals(EnterExit weekTotals)
        {
            var lunchTime = GetSumOfTime(weekTotals.EnterLunchTimers, weekTotals.ExitLunchTimers);

            return lunchTime;
        }
    }
}
