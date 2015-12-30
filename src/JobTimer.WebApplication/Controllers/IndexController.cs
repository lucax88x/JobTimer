using System.Web.Mvc;

namespace JobTimer.WebApplication.Controllers
{
    public class IndexController : BaseController
    {                
        public ActionResult Index()
        {
            return HandleViewForSpa();
        }                
        public ActionResult Timer()
        {
            return HandleViewForSpa();
        }
    }
}