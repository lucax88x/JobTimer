using System.ComponentModel.DataAnnotations;

namespace JobTimer.WebApplication.ViewModels.WebApi
{
    public class TypeAheadBindingModel
    {
        [Required]
        public int Limit { get; set; }
        [Required]
        public string Query { get; set; }
    }
}