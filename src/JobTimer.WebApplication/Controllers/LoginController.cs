using System.Web.Mvc;

namespace JobTimer.WebApplication.Controllers
{
    public class LoginController : Controller
    {        
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("/");
            }            
        }
    }
}