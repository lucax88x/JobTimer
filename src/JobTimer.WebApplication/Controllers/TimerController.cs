using System.Web.Mvc;

namespace JobTimer.WebApplication.Controllers
{
    public class TimerController : BaseController
    {                
        public ActionResult Charts()
        {
            return HandleViewForSpa();
        }                
    }
}