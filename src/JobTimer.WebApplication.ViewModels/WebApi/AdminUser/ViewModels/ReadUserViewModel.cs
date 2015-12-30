using JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.User;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.ViewModels
{
    public class ReadUserViewModel : BaseViewModel
    {
        public UserJson Data { get; set; }
    }
}