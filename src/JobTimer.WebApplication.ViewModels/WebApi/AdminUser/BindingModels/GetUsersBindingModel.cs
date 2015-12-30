using System.ComponentModel.DataAnnotations;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.BindingModels
{    
    public class GetUsersBindingModel
    {        
        [Required]
        public int start { get; set; }

        [Required]
        public int limit { get; set; }
    }
}