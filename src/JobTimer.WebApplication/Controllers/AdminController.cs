using System.Web.Mvc;

namespace JobTimer.WebApplication.Controllers
{
    public class AdminController : BaseController
    {
        [Authorize(Order = 0, Roles = "Admin")]
        public ActionResult User()
        {
            return HandleViewForSpa();
        }
    }
}