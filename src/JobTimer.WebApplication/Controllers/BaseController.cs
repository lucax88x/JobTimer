using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace JobTimer.WebApplication.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationRoleManager _roleManager;

        public BaseController()
        {

        }

        public BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    if (Request != null)
                    {
                        _userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    }
                }
                return _userManager;

            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                if (_roleManager == null)
                {
                    if (Request != null)
                    {
                        _roleManager = Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
                    }
                }
                return _roleManager;

            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    if (Request != null)
                    {
                        _signInManager = Request.GetOwinContext().Get<ApplicationSignInManager>();
                    }
                }
                return _signInManager;

            }
            private set
            {
                _signInManager = value;
            }
        }

        public string UserName => User.Identity.Name;

        public IList<string> Roles
        {
            get
            {
                var user = UserManager.FindByName(UserName);
                return UserManager.GetRoles(user.Id);
            }
        }

        protected ActionResult HandleViewForSpa(string view = "")
        {
            if (Request.IsAuthenticated)
            {
                if (Request.QueryString["SPA"] == "1")
                {
                    return View(view);
                }
                else
                {
                    return View("~/Views/Master/Empty.cshtml");
                }
            }
            else
            {
                return Redirect("/Login");
            }
        }
    }
}
