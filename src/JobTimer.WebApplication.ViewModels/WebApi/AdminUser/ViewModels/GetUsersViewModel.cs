using JobTimer.WebApplication.ViewModels.WebApi.AdminUser.Models;
using JobTimer.WebApplication.ViewModels.WebApi.Common;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.ViewModels
{
    public class GetUsersViewModel : BaseViewModel
    {
        public GridData<UserGridJson> data { get; set; }

        public GetUsersViewModel()
        {
            data = new GridData<UserGridJson>();
        }
    }
}