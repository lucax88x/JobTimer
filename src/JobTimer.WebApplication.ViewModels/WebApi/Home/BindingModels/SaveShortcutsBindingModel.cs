using System.Collections.Generic;
using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Home.BindingModels
{
    [TsClass(Module = Modules.BindingModels.Home)]
    public class SaveShortcutsBindingModel
    {
        public IEnumerable<string> Shortcuts { get; set; }
    }
}