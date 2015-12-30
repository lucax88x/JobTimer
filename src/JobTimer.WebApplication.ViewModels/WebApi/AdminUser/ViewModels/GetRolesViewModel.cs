using System.Collections.Generic;
using JobTimer.WebApplication.TypeScript;
using JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.Role;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.ViewModels
{
    [TsClass(Module = Modules.ViewModels.AdminUser)]
    public class GetRolesViewModel : BaseViewModel
    {
        public List<RoleSimpleJson> Roles { get; set; }

        public GetRolesViewModel()
        {
            Roles = new List<RoleSimpleJson>();
        }
    }
}