using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Utils;
using JobTimer.WebApplication.Code;
using JobTimer.WebApplication.Logic;
using JobTimer.WebApplication.TypeScript;
using JobTimer.WebApplication.ViewModels.WebApi.Timer.BindingModels;
using JobTimer.WebApplication.ViewModels.WebApi.Timer.ViewModels;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class TimerController : BaseApiController
    {
        private readonly ITimerAccess _timerAccess;
        private readonly IRequestReader _requestReader;
        private readonly ITimeBuilder _timeBuilder;
        private readonly TimeZoneInfo _timeZoneInfo;
        private readonly ITimerCalculator _timerCalculator;
        private readonly IOvertimeAccess _overtimeAccess;

        public TimerController(ITimerAccess timerAccess, IRequestReader requestReader, ITimeBuilder timeBuilder, ITimerCalculator timerCalculator, IOvertimeAccess overtimeAccess)
        {
            _timerAccess = timerAccess;
            _requestReader = requestReader;
            _timeBuilder = timeBuilder;
            _timerCalculator = timerCalculator;
            _overtimeAccess = overtimeAccess;

            _timeZoneInfo = _timeBuilder.GetTimeZoneInfo(_requestReader.Read(HttpHeaders.Request.CurrentTimeZone));
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> GetStatus(GetStatusBindingModel model)
        {
            var result = new GetStatusViewModel();

            var date = TimeZoneInfo.ConvertTimeToUtc(model.Date.Date);
            result.Status = await ElaborateStatus(date);
            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SaveEnter(SaveEnterBindingModel model)
        {
            var result = new SaveEnterViewModel();

            var date = _timeBuilder.UpdateTime(model.Date, DateTime.UtcNow.TimeOfDay);
            var timer = GetTimer(TimerTypes.Enter, date, model.Offset);

            var lastExit = await _timerAccess.GetLastAsyncOfDate(UserName, new List<int> { (int)TimerTypes.Exit }, date);

            if (lastExit == null || ((timer.Date.DayOfYear == lastExit.Date.DayOfYear && timer.Time > lastExit.Time)))
            {
                result.Date = timer.Date;
                result.Result = await _timerAccess.SaveOrUpdateAsync(timer) > 0;
                result.Status = await ElaborateStatus(date);
            }
            else
            {
                return BadRequest(string.Format("Date used is before the last one {0}!", _timeBuilder.ConvertTimeSpanToTimeZoneInfo(lastExit.Time, _timeZoneInfo).ToString("hh':'mm")));
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SaveEnterLunch(SaveEnterLunchBindingModel model)
        {
            var result = new SaveEnterLunchViewModel();

            var date = _timeBuilder.UpdateTime(model.Date, DateTime.UtcNow.TimeOfDay);
            var timer = GetTimer(TimerTypes.EnterLunch, date, model.Offset);

            var lastExitLunch = await _timerAccess.GetLastAsyncOfDate(UserName, new List<int> { (int)TimerTypes.ExitLunch }, date);

            if (lastExitLunch == null || ((timer.Date.DayOfYear == lastExitLunch.Date.DayOfYear && timer.Time > lastExitLunch.Time)))
            {
                result.Date = timer.Date;
                result.Result = await _timerAccess.SaveOrUpdateAsync(timer) > 0;
                result.Status = await ElaborateStatus(date);
            }
            else
            {
                return BadRequest(string.Format("Date used is before the last one {0}!", _timeBuilder.ConvertTimeSpanToTimeZoneInfo(lastExitLunch.Time, _timeZoneInfo).ToString("hh':'mm")));
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SaveExitLunch(SaveExitLunchBindingModel model)
        {
            var result = new SaveExitLunchViewModel();

            var date = _timeBuilder.UpdateTime(model.Date, DateTime.UtcNow.TimeOfDay);
            var timer = GetTimer(TimerTypes.ExitLunch, date, model.Offset);

            var lastEnterLunch = await _timerAccess.GetLastAsyncOfDate(UserName, new List<int> { (int)TimerTypes.EnterLunch }, date);

            if (lastEnterLunch == null || ((timer.Date.DayOfYear == lastEnterLunch.Date.DayOfYear && timer.Time > lastEnterLunch.Time)))
            {
                result.Date = timer.Date;
                result.Result = await _timerAccess.SaveOrUpdateAsync(timer) > 0;
                result.Status = await ElaborateStatus(date);
            }
            else
            {
                return BadRequest(string.Format("Date used is before the last one {0}!", _timeBuilder.ConvertTimeSpanToTimeZoneInfo(lastEnterLunch.Time, _timeZoneInfo).ToString("hh':'mm")));
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SaveExit(SaveExitBindingModel model)
        {
            var result = new SaveExitViewModel();

            var date = _timeBuilder.UpdateTime(model.Date, DateTime.UtcNow.TimeOfDay);
            var timer = GetTimer(TimerTypes.Exit, date, model.Offset);

            var lastEnter = await _timerAccess.GetLastAsyncOfDate(UserName, new List<int> { (int)TimerTypes.Enter }, date);

            if (lastEnter == null || ((timer.Date.DayOfYear == lastEnter.Date.DayOfYear && timer.Time > lastEnter.Time)))
            {
                result.Date = timer.Date;
                result.Result = await _timerAccess.SaveOrUpdateAsync(timer) > 0;
                result.Status = await ElaborateStatus(date);

                await SaveOvertime(date);
            }
            else
            {
                return BadRequest(string.Format("Date used is before the last one {0}!", _timeBuilder.ConvertTimeSpanToTimeZoneInfo(lastEnter.Time, _timeZoneInfo).ToString("hh':'mm")));
            }

            return Ok(result);
        }

        private async Task SaveOvertime(DateTime date)
        {
            var overtime = await _timerCalculator.CalculateOvertime(UserName, date);

            if (Math.Abs(overtime.TotalMilliseconds) > 0)
            {
                var alreadyExistingOvertime = await _overtimeAccess.LoadAsync(UserName, date);

                if (alreadyExistingOvertime != null)
                    await _overtimeAccess.DeleteAsync(alreadyExistingOvertime);

                await _overtimeAccess.SaveOrUpdateAsync(new Overtime()
                {
                    UserName = UserName,
                    Date = DateTime.Now,
                    Time = overtime.Ticks
                });
            }
        }

        private Timer GetTimer(TimerTypes timerType, DateTime date, int offset)
        {
            var timer = new Timer
            {
                Date = date.Date,
                Time = _timeBuilder.RemoveSeconds(date.AddMinutes(offset).TimeOfDay),
                UserName = UserName,
                TimerTypeId = (int)timerType
            };
            return timer;
        }

        private async Task<TimerStatus> ElaborateStatus(DateTime date)
        {
            var status = new TimerStatus();

            var timer = await _timerAccess.GetLastAsyncOfDate(UserName, new List<int> { (int)TimerTypes.Enter, (int)TimerTypes.EnterLunch, (int)TimerTypes.Exit, (int)TimerTypes.ExitLunch }, date);

            if (timer != null)
            {
                switch (timer.TimerTypeId)
                {
                    case (int)TimerTypes.Enter:
                        status.EnterLunch = true;
                        status.Exit = true;
                        break;
                    case (int)TimerTypes.EnterLunch:
                        status.ExitLunch = true;
                        status.Exit = true;
                        break;
                    case (int)TimerTypes.ExitLunch:
                        status.EnterLunch = true;
                        status.Exit = true;
                        break;
                    case (int)TimerTypes.Exit:
                        status.Enter = true;
                        break;
                }
            }
            else
            {
                status.Enter = true;
            }
            return status;
        }
    }
}