using JobTimer.Utils;
using JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.Role;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JobTimer.WebApplication.CopyPresets.Identity
{
    public interface IRoleCopier
    {
        void CopyRole(IdentityRole source, RoleSimpleJson target);
        void CopyRole(RoleSimpleJson source, IdentityRole target);
    }

    public class RoleCopier : Copier, IRoleCopier
    {
        public void CopyRole(IdentityRole source, RoleSimpleJson target)
        {
            Copy(source, target);
        }
        public void CopyRole(RoleSimpleJson source, IdentityRole target)
        {
            Copy(source, target);
        }
    }
}