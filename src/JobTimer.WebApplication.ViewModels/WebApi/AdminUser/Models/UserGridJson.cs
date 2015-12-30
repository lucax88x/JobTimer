using JobTimer.WebApplication.ViewModels.WebApi.Common;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.Models
{
    public class UserGridJson : ReadJson<string>
    {        
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}