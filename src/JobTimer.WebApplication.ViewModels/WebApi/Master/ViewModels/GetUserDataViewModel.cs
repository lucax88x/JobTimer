using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Master.ViewModels
{
    [TsClass(Module = Modules.ViewModels.Master)]
    public class GetUserDataViewModel : UserDataViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}