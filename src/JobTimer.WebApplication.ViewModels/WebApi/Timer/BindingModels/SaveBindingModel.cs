using System;
using System.ComponentModel.DataAnnotations;
using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Timer.BindingModels
{
    [TsClass(Module = Modules.BindingModels.Timer)]
    public class SaveBindingModel
    {
        [Required]
        public int Offset { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}