using System.ComponentModel.DataAnnotations;
using JobTimer.WebApplication.ViewModels.WebApi.AdminUser.Models;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.BindingModels
{
    public class DeleteUserBindingModel
    {
        [Required]
        public UserSimpleJson Data { get; set; }
    }
}