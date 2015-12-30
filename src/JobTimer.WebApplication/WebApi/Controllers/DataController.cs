using System.Web.Http;
using JobTimer.WebApplication.Loaders.Navigation;
using JobTimer.WebApplication.ViewModels.WebApi.Data.BindingModels;
using JobTimer.WebApplication.ViewModels.WebApi.Data.Models.Search;
using JobTimer.WebApplication.ViewModels.WebApi.Data.ViewModels;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class DataController : BaseApiController
    {        
        readonly IMenuLoader _menuLoader;     

        public DataController(IMenuLoader menuLoader)
        {            
            _menuLoader = menuLoader;            
        }

        [HttpPost]
        [Authorize(Roles = "Admin, TimerUser")]
        public IHttpActionResult SearchPages(SearchPagesBindingModel model)
        {
            var result = new SearchPagesViewModel();

            var items = _menuLoader.GetAllChildsFiltered(Roles, model.Query, model.Limit);

            foreach (var item in items)
            {
                result.Pages.Add(new SearchItem() { Id = item.Id, Text = item.Name, Url = item.Url, Type = SearchItemType.Page });
            }

            result.Result = true;

            return Ok(result);
        }
    }
}