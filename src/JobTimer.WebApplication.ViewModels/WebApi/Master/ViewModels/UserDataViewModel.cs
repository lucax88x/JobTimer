using System;
using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Master.ViewModels
{
    [TsEnum(Module = Modules.ViewModels.Master)]
    public enum OffsetTypes
    {
        Positive = 1,
        Negative = -1,
        Neutral = 0
    }

    [TsClass(Module = Modules.ViewModels.Master)]
    public class UserDataViewModel : BaseViewModel
    {
        public string TimeOffset { get; set; }        
        public OffsetTypes TimeOffsetType { get; set;  }
        public bool HasEstimatedExitTime { get; set; }
        public DateTime EstimatedExitTime { get; set; }
    }
}