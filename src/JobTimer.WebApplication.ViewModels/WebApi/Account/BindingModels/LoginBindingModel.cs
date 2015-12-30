using System.ComponentModel.DataAnnotations;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Account.BindingModels
{
    [TsClass(Module = TypeScript.Modules.BindingModels.Account)]
    public class LoginBindingModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}