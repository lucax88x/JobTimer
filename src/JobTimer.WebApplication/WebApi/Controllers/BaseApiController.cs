using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Security;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationRoleManager _roleManager;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        public BaseApiController()
        {
        }

        public BaseApiController(
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            ApplicationRoleManager roleManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            AccessTokenFormat = accessTokenFormat;
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

        public IHubContext<Y> GetHub<T, Y>() where T : IHub where Y : class
        {
            return GlobalHost.ConnectionManager.GetHubContext<T, Y>();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && Request != null)
            {
                _userManager?.Dispose();
                _roleManager?.Dispose();
                _signInManager?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}