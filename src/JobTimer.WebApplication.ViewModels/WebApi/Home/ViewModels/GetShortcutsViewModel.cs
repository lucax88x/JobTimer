using System.Collections.Generic;
using JobTimer.WebApplication.TypeScript;
using JobTimer.WebApplication.ViewModels.Common; 
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Home.ViewModels
{
    [TsClass(Module = Modules.ViewModels.Home)]
    public class GetShortcutsViewModel : BaseViewModel
    {
        public List<MenuItem> Items { get; set; }        

        public GetShortcutsViewModel()
        {
            Items = new List<MenuItem>();            
        }
    }
}