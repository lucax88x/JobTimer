using System.Text;
using System.Web.Mvc;
using JobTimer.Data.Access.JobTimer;
using JobTimer.WebApplication.Loaders.Navigation;
using JobTimer.WebApplication.ViewModels.Common;

namespace JobTimer.WebApplication.Controllers
{
    public class MasterController : BaseController
    {
        private readonly IMenuLoader _menuLoader;
        private readonly ILastVisitedAccess _lastVisitedAccess;
        public MasterController(IMenuLoader menuLoader, ILastVisitedAccess lastVisitedAccess)
        {
            _menuLoader = menuLoader;
            _lastVisitedAccess = lastVisitedAccess;
        }

        public ActionResult UserData()
        {
            return View();
        }
        public ActionResult TopNavBar()
        {
            var topNavBar = new TopNavBar { Admin = User.IsInRole("Admin") };
            return View(topNavBar);
        }
        public ActionResult Analytics()
        {
#if DEBUG
            return new EmptyResult();
#else
            return View();            
#endif

        }
        public ActionResult Menu()
        {
            var menu = _menuLoader.Load(Roles);

            return View(menu);
        }
        public ActionResult LastVisiteds()
        {
            var lastVisited = new LastVisited();
            var urls = _lastVisitedAccess.Load(UserName);

            if (urls != null)
            {
                var sb = new StringBuilder();
                sb.Append("[");
                foreach (var url in urls.Visited.Split('|'))
                {
                    sb.AppendFormat("'{0}',", url);
                }

                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("]");

                lastVisited.Urls = sb.ToString();
            }
            else
            {
                lastVisited.Urls = string.Empty;
            }

            return View(lastVisited);
        }
        public ActionResult ExitTimeEstimated()
        {
            return View();
        }
    }
}