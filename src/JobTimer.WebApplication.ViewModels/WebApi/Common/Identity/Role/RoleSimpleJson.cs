using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.Role
{
    [TsClass(Module = Modules.Models.Role)]
    public class RoleSimpleJson
    {
        public string Id { get; set; }
        public string Name { get; set; }        
    }
}