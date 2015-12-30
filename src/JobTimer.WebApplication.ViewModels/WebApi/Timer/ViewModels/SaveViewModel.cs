using System;
using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Timer.ViewModels
{
    [TsClass(Module = Modules.ViewModels.Timer)]
    public class SaveViewModel : GetStatusViewModel
    {
        public DateTime Date { get; set; }
    }
}