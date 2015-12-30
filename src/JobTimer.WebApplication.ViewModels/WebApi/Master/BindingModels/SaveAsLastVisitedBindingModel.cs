using System.ComponentModel.DataAnnotations;

namespace JobTimer.WebApplication.ViewModels.WebApi.Master.BindingModels
{
    public class SaveAsLastVisitedBindingModel
    {
        [Required]
        public string Url { get; set; }
    }
}