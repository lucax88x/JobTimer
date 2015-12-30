using System.Collections.Generic;

namespace JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.User
{
    public class SaveUserJson : UserJson
    {
        public string Password { get; set; }
    }
    public class UserJson
    {
        public string Id { get; set; }
        public string UserName { get; set; }        
        public string Email { get; set; }
        public List<string> Roles { get; set; }        

        public UserJson()
        {
            Roles = new List<string>();            
        }
    }
}