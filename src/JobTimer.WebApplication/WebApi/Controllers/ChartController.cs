using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Dtos.Timer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Utils;
using JobTimer.WebApplication.Code;
using JobTimer.WebApplication.Logic;
using JobTimer.WebApplication.ViewModels.WebApi.Chart.ViewModels;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class ChartController : BaseApiController
    {
        private readonly ITimerAccess _timerAccess;
        private readonly ITimeBuilder _timeBuilder;
        private readonly ITimeGetter _timeGetter;
        private readonly IRequestReader _requestReader;
        private readonly TimeZoneInfo _timeZoneInfo;
        private readonly ITimerCalculator _timerCalculator;

        public ChartController(ITimerAccess timerAccess, ITimeBuilder timeBuilder, ITimeGetter timeGetter, IRequestReader requestReader, ITimerCalculator timerCalculator)
        {
            _timerAccess = timerAccess;
            _timeBuilder = timeBuilder;
            _timeGetter = timeGetter;
            _requestReader = requestReader;
            _timerCalculator = timerCalculator;

            _timeZoneInfo = _timeBuilder.GetTimeZoneInfo(_requestReader.Read(TypeScript.HttpHeaders.Request.CurrentTimeZone));
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetWeeklyTimes()
        {
            var result = new GetWeeklyTimesViewModel();

            var timers = await _timerAccess.GetRangesOfThisWeek(UserName);

            var chartSerie = GetWeeklyTimesSerie(timers);

            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetMonthlyTimes()
        {
            var result = new GetMonthlyTimesViewModel();

            var timers = await _timerAccess.GetRangesOfThisMonth(UserName);

            var chartSerie = GetMonthlyTimesSerie(timers);
            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }


        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetWeeklyTotals()
        {
            var result = new GetWeeklyTotalsViewModel();

            var totals = await _timerCalculator.CalculateTotalsOfThisWeek(UserName);

            var chartSerie = GetWeeklyTotalsSerie(totals);

            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetMonthlyTotals()
        {
            var result = new GetMonthlyTotalsViewModel();

            var totals = await _timerCalculator.CalculateTotalsOfThisMonth(UserName);

            var chartSerie = GetMonthlyTotalsSerie(totals);

            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetWeeklyLunchTimes()
        {
            var result = new GetWeeklyLunchTimesViewModel();

            var timers = await _timerAccess.GetRangesOfLunchOfThisWeek(UserName);

            var chartSerie = GetWeeklyTimesSerie(timers);

            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetMonthlyLunchTimes()
        {
            var result = new GetMonthlyLunchTimesViewModel();

            var timers = await _timerAccess.GetRangesOfLunchOfThisMonth(UserName);

            var chartSerie = GetMonthlyTimesSerie(timers);
            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetWeeklyLunchTotals()
        {
            var result = new GetWeeklyLunchTotalsViewModel();

            var totals = await _timerCalculator.CalculateTotalsOfLunchOfThisWeek(UserName);

            var chartSerie = GetWeeklyTotalsSerie(totals);

            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetMonthlyLunchTotals()
        {
            var result = new GetMonthlyLunchTotalsViewModel();

            var totals = await _timerCalculator.CalculateTotalsOfLunchOfThisMonth(UserName);

            var chartSerie = GetMonthlyTotalsSerie(totals);

            result.Data.series.Add(chartSerie);

            result.Result = true;

            return Ok(result);
        }

        #region private
        private Tuple<DateTime?, DateTime?> CalculateExtremes(List<DateTime?> inst)
        {
            if (!inst[0].HasValue && !inst[1].HasValue)
            {
                return new Tuple<DateTime?, DateTime?>(null, null);
            }

            if (inst[0].HasValue && inst[1].HasValue)
            {
                if (inst[0] > inst[1])
                {
                    inst[1] = inst[0].Value.AddMinutes(5);
                }
                else if (inst[1] < inst[0])
                {
                    inst[1] = inst[0].Value.AddMinutes(5);
                }
            }
            else if (!inst[0].HasValue)
            {
                inst[0] = inst[1].Value.AddMinutes(-5);
            }
            else if (!inst[1].HasValue)
            {
                inst[1] = inst[0].Value.AddMinutes(5);
            }

            return new Tuple<DateTime?, DateTime?>(inst[0].Value, inst[1].Value);
        }

        private ChartSerie<List<long?>> GetWeeklyTimesSerie(EnterExitRanges timers)
        {
            var dict = _timeBuilder.GetDayOfWeeks()
                .ToDictionary(dayOfWeek => dayOfWeek, dayOfWeek => new List<DateTime?> { null, null });

            foreach (var timer in timers.EnterTimers)
            {
                dict[timer.Date.DayOfWeek][0] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            foreach (var timer in timers.ExitTimers)
            {
                dict[timer.Date.DayOfWeek][1] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            var chartSerie = new ChartSerie<List<long?>>();
            foreach (var dayOfWeek in _timeBuilder.GetDayOfWeeks())
            {
                var inst = dict[dayOfWeek];

                var extremes = CalculateExtremes(inst);

                if (extremes.Item1.HasValue && extremes.Item2.HasValue)
                {
                    chartSerie.data.Add(new List<long?>()
                    {
                        _timeBuilder.ToJavascriptTicks(extremes.Item1.Value, _timeZoneInfo),
                        _timeBuilder.ToJavascriptTicks(extremes.Item2.Value, _timeZoneInfo)
                    });
                }
                else
                {
                    chartSerie.data.Add(new List<long?> { null, null });
                }
            }
            return chartSerie;
        }

        private ChartSerie<List<long?>> GetMonthlyTimesSerie(EnterExitRanges timers)
        {
            var dict = new Dictionary<int, List<DateTime?>>();
            for (var i = 1; i < _timeBuilder.GetMonths(); i++)
            {
                dict.Add(i, new List<DateTime?> { null, null });
            }

            foreach (var timer in timers.EnterTimers)
            {
                dict[timer.Date.Day][0] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            foreach (var timer in timers.ExitTimers)
            {
                dict[timer.Date.Day][1] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            var chartSerie = new ChartSerie<List<long?>>();
            for (var i = 1; i < _timeBuilder.GetMonths(); i++)
            {
                var inst = dict[i];

                var extremes = CalculateExtremes(inst);

                if (extremes.Item1.HasValue && extremes.Item2.HasValue)
                {
                    chartSerie.data.Add(new List<long?>()
                    {
                        _timeBuilder.ToJavascriptTicks(extremes.Item1.Value,
                            _timeZoneInfo),
                        _timeBuilder.ToJavascriptTicks(extremes.Item2.Value,
                            _timeZoneInfo)
                    });
                }
                else
                {
                    chartSerie.data.Add(new List<long?> { null, null });
                }
            }
            return chartSerie;
        }

        private ChartSerie<double?> GetWeeklyTotalsSerie(Dictionary<DateTime, TimeSpan> totals)
        {
            var chartSerie = new ChartSerie<double?>();

            var groupedByDayOfWeek = totals.GroupBy(x => x.Key.DayOfWeek).ToDictionary(x => x.Key, x => x.Select(y => y.Value).FirstOrDefault());

            foreach (var dayOfWeek in _timeBuilder.GetDayOfWeeks())
            {
                if (groupedByDayOfWeek.ContainsKey(dayOfWeek))
                {
                    var totalsByDay = groupedByDayOfWeek[dayOfWeek];
                    chartSerie.data.Add(totalsByDay.Hours + (totalsByDay.Minutes / 100d));
                }
                else
                {
                    chartSerie.data.Add(null);
                }
            }
            return chartSerie;
        }

        private ChartSerie<double?> GetMonthlyTotalsSerie(Dictionary<DateTime, TimeSpan> totals)
        {
            var chartSerie = new ChartSerie<double?>();

            var groupedByDayOfWeek = totals.GroupBy(x => x.Key.Day).ToDictionary(x => x.Key, x => x.Select(y => y.Value).FirstOrDefault());

            for (var i = 1; i < _timeBuilder.GetMonths(); i++)
            {
                if (groupedByDayOfWeek.ContainsKey(i))
                {
                    var totalsByDay = groupedByDayOfWeek[i];
                    chartSerie.data.Add(totalsByDay.Hours + (totalsByDay.Minutes / 100d));
                }
                else
                {
                    chartSerie.data.Add(null);
                }
            }
            return chartSerie;
        }

        private ChartSerie<double?> GetWeeklyLunchTotalsSerie(EnterExitRanges timers)
        {
            var enterExitsLunch = _timeBuilder.GetDayOfWeeks().ToDictionary(dayOfweek => dayOfweek, dayOfweek => new List<DateTime?>() { null, null });

            foreach (var timer in timers.EnterTimers)
            {
                enterExitsLunch[timer.Date.DayOfWeek][0] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            foreach (var timer in timers.ExitTimers)
            {
                enterExitsLunch[timer.Date.DayOfWeek][1] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            var chartSerie = new ChartSerie<double?>();

            foreach (var dayOfWeek in _timeBuilder.GetDayOfWeeks())
            {
                var enterExitLunch = enterExitsLunch[dayOfWeek];
                var enterExitLunchExtremes = CalculateExtremes(enterExitLunch);

                if (enterExitLunchExtremes.Item1.HasValue && enterExitLunchExtremes.Item2.HasValue)
                {
                    var enterExitLunchDiff = (enterExitLunchExtremes.Item2.Value - enterExitLunchExtremes.Item1.Value);
                    chartSerie.data.Add(enterExitLunchDiff.Hours + (enterExitLunchDiff.Minutes / 100d));
                }
                else
                {
                    chartSerie.data.Add(null);
                }
            }
            return chartSerie;
        }

        private ChartSerie<double?> GetMonthlyLunchTotalsSerie(EnterExitRanges timers)
        {
            var enterExitsLunch = new Dictionary<int, List<DateTime?>>();

            for (var i = 1; i < _timeBuilder.GetMonths(); i++)
            {
                enterExitsLunch.Add(i, new List<DateTime?>() { null, null });
            }

            foreach (var timer in timers.EnterTimers)
            {
                enterExitsLunch[timer.Date.Day][0] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            foreach (var timer in timers.ExitTimers)
            {
                enterExitsLunch[timer.Date.Day][1] = _timeBuilder.UpdateTime(_timeGetter.Now, timer.Time);
            }

            var chartSerie = new ChartSerie<double?>();

            for (var i = 1; i < _timeBuilder.GetMonths(); i++)
            {
                var enterExitLunch = enterExitsLunch[i];
                var enterExitLunchExtremes = CalculateExtremes(enterExitLunch);

                if (enterExitLunchExtremes.Item1.HasValue && enterExitLunchExtremes.Item2.HasValue)
                {
                    var enterExitLunchDiff = (enterExitLunchExtremes.Item2.Value - enterExitLunchExtremes.Item1.Value);
                    chartSerie.data.Add(enterExitLunchDiff.Hours + (enterExitLunchDiff.Minutes / 100d));
                }

                else
                {
                    chartSerie.data.Add(null);
                }
            }
            return chartSerie;
        }
        #endregion
    }
}