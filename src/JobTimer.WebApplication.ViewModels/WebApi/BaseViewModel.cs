using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi
{
    [TsClass(Module = Modules.ViewModels.Name)]
    public class BaseViewModel
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }    
}
