using JobTimer.Data.Model.Identity;
using JobTimer.Utils;
using JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.User;

namespace JobTimer.WebApplication.CopyPresets.Identity
{
    public interface IUserCopier
    {
        void CopyUser(ApplicationUser source, UserJson target);
        void CopyUser(UserJson source, ApplicationUser target);
    }

    public class UserCopier : Copier, IUserCopier
    {
        public void CopyUser(ApplicationUser source, UserJson target)
        {
            Copy(source, target);
        }
        public void CopyUser(UserJson source, ApplicationUser target)
        {
            Copy(source, target);
        }
    }
}