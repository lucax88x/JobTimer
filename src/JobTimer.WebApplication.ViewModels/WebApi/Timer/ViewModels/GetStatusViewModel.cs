using System.Collections.Generic;
using JobTimer.WebApplication.TypeScript;
using JobTimer.WebApplication.ViewModels.Common;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Timer.ViewModels
{
    [TsClass(Module = Modules.ViewModels.Timer)]
    public class TimerStatus
    {
        public bool Enter { get; set; }
        public bool EnterLunch { get; set; }
        public bool ExitLunch { get; set; }
        public bool Exit { get; set; }
    }
    [TsClass(Module = Modules.ViewModels.Timer)]
    public class GetStatusViewModel : BaseViewModel
    {
        public TimerStatus Status { get; set; }

        public GetStatusViewModel()
        {
            Status = new TimerStatus();
        }
    }
}