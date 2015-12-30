using System;
using System.ComponentModel.DataAnnotations;
using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Timer.BindingModels
{
    [TsClass(Module = Modules.BindingModels.Timer)]
    public class GetStatusBindingModel
    {
        [Required]
        public DateTime Date { get; set; }
    }
}