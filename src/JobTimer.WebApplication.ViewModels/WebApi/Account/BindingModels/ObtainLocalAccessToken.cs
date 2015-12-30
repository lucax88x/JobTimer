using System.ComponentModel.DataAnnotations;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Account.BindingModels
{
    [TsClass(Module = TypeScript.Modules.BindingModels.Account)]
    public class VerifyExternalLoginBindingModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }

    }
}