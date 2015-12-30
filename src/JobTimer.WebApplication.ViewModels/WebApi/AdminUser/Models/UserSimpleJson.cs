using JobTimer.WebApplication.ViewModels.WebApi.Common;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.Models
{
    public class UserSimpleJson : ReadJson<string>
    {        
        public string UserName { get; set; }        
    }
}