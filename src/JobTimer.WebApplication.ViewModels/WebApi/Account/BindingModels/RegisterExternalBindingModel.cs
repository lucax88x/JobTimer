using System.ComponentModel.DataAnnotations;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Account.BindingModels
{
    [TsClass(Module = TypeScript.Modules.BindingModels.Account)]
    public class RegisterExternalBindingModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }

    }
}