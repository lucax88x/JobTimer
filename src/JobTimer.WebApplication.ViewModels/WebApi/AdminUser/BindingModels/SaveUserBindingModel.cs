using System.ComponentModel.DataAnnotations;
using JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.User;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.BindingModels
{
    public class SaveUserBindingModel
    {
        [Required]
        public SaveUserJson Data { get; set; }
    }
}