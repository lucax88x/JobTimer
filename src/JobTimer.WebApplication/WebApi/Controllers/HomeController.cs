using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.WebApplication.Loaders.Navigation;
using JobTimer.WebApplication.ViewModels.WebApi.Home.BindingModels;
using JobTimer.WebApplication.ViewModels.WebApi.Home.ViewModels;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class HomeController : BaseApiController
    {
        private readonly IMenuLoader _menuLoader;
        private readonly IShortcutAccess _shortcutAccess;        

        public HomeController(
            IMenuLoader menuLoader,
            IShortcutAccess shortcutAccess)
        {
            _menuLoader = menuLoader;
            _shortcutAccess = shortcutAccess;            
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetShortcuts()
        {            
            var result = new GetShortcutsViewModel();

            var user = await UserManager.FindByNameAsync(UserName);
            var roles = await UserManager.GetRolesAsync(user.Id);
            var menu = _menuLoader.GetAllChilds(roles);

            var shortcuts = await _shortcutAccess.LoadAsync(UserName);

            if (shortcuts != null)
            {
                result.Items = (from id in shortcuts.Shortcuts.Split('|')
                                join m in menu
                                on id equals m.Id
                                select m).ToList();
            }
            else
            {
                result.Items = menu;
            }            

            result.Result = true;

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SaveShortcuts(SaveShortcutsBindingModel model)
        {
            var result = new SaveShortcutsViewModel();

            var shortcuts = await _shortcutAccess.LoadAsync(UserName);

            if (shortcuts == null)
            {
                shortcuts = new Shortcut() { UserName = UserName };
            }

            shortcuts.Shortcuts = string.Join("|", model.Shortcuts);
            await _shortcutAccess.SaveOrUpdateAsync(shortcuts);

            result.Result = true;

            return Ok(result);
        }
    }
}