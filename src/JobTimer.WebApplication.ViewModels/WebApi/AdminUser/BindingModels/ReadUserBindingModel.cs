using System.ComponentModel.DataAnnotations;
using JobTimer.WebApplication.ViewModels.WebApi.Common;

namespace JobTimer.WebApplication.ViewModels.WebApi.AdminUser.BindingModels
{
    public class ReadUserBindingModel
    {
        [Required]
        public ReadJson<string> Data { get; set; }
    }
}