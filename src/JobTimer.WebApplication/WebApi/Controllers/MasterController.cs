using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Optimization;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Utils;
using JobTimer.WebApplication.Code;
using JobTimer.WebApplication.Loaders.Master;
using JobTimer.WebApplication.Logic;
using JobTimer.WebApplication.ViewModels.WebApi.Master.BindingModels;
using JobTimer.WebApplication.ViewModels.WebApi.Master.ViewModels;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class MasterController : BaseApiController
    {
        private readonly IBundlesLoader _bundlesLoader;
        private readonly ILastVisitedAccess _lastVisitedAccess;
        private readonly ITimerCalculator _timerCalculator;
        private readonly IOvertimeAccess _overtimeAccess;

        public MasterController(IBundlesLoader bundlesLoader, ILastVisitedAccess lastVisitedAccess, ITimerCalculator timerCalculator, IOvertimeAccess overtimeAccess)
        {
            _bundlesLoader = bundlesLoader;
            _lastVisitedAccess = lastVisitedAccess;
            _timerCalculator = timerCalculator;
            _overtimeAccess = overtimeAccess;
        }

        public IHttpActionResult GetBundles()
        {
            var result = new GetBundlesViewModel();
            var bundles = _bundlesLoader.Load();

            foreach (var bundle in bundles.Bundles)
            {
                var bundleItem = new BundleItem();
                bundleItem.Bundle = bundle.Bundle;

                var rendered = Scripts.Render(string.Format("~/bundles/{0}", bundle.Bundle)).ToString();
                var matchedScripts = new Regex(@"src=([""'])(.*?)\1");
                foreach (Match script in matchedScripts.Matches(rendered))
                {
                    if (script.Groups.Count > 1)
                    {
                        bundleItem.Scripts.Add(script.Groups[2].Value);
                    }
                }

                result.Bundles.Add(bundleItem);
            }

            return Ok(result);
        }

        [HttpPost]
        [System.Web.Http.Authorize]
        public async Task<IHttpActionResult> SaveAsLastVisited(SaveAsLastVisitedBindingModel model)
        {
            var result = new SaveAsLastVisitedViewModel();

            var visiteds = await _lastVisitedAccess.LoadAsync(UserName);

            if (visiteds != null)
            {
                var splitted = visiteds.Visited.Split('|').ToList();

                if (splitted.Contains(model.Url))
                {
                    splitted.Remove(model.Url);
                }

                var final = new List<string> { model.Url };
                final.AddRange(splitted);
                visiteds.Visited = string.Join("|", final.Take(5));
            }
            else
            {
                visiteds = new LastVisited() { UserName = UserName, Visited = model.Url };
            }

            await _lastVisitedAccess.SaveOrUpdateAsync(visiteds);

            result.Result = true;

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserData()
        {
            var result = new GetUserDataViewModel();

            var user = await UserManager.FindByNameAsync(UserName);

            result.UserName = UserName;
            result.Email = user.Email;

            await GetUserData(result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IHttpActionResult> UpdateUserData()
        {
            var result = new UpdateUserDataViewModel();

            await GetUserData(result);

            return Ok(result);
        }


        private async Task GetUserData(UserDataViewModel result)
        {
            var estimation = await _timerCalculator.CalculateEstimation(UserName);

            if (estimation != null)
            {
                result.EstimatedExitTime = estimation.EstimatedExitTime;
                result.HasEstimatedExitTime = estimation.HasEstimatedExitTime;
            }

            var overtime = await _overtimeAccess.SumAllOvertimesAsync(UserName);

            if (overtime.HasValue)
            {
                var offset = TimeSpan.FromTicks(overtime.Value);

                if (offset.TotalMilliseconds > 0)
                {
                    result.TimeOffsetType = OffsetTypes.Positive;
                    result.TimeOffset = "+";
                }
                else if (offset.TotalMilliseconds < 0)
                {
                    result.TimeOffsetType = OffsetTypes.Negative;
                    result.TimeOffset = "-";
                }
                else
                {
                    result.TimeOffsetType = OffsetTypes.Neutral;
                }

                result.TimeOffset += offset.ToString("hh':'mm");
            }
            else
            {
                result.TimeOffsetType = OffsetTypes.Neutral;
                result.TimeOffset = "00:00";
            }

            result.Result = true;
        }

    }
}