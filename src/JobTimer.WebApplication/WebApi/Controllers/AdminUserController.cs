using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using JobTimer.Data.Access.Identity;
using JobTimer.Data.Model.Identity;
using JobTimer.WebApplication.Code;
using JobTimer.WebApplication.CopyPresets.Identity;
using JobTimer.WebApplication.Hubs;
using JobTimer.WebApplication.Hubs.Models.Notification;
using JobTimer.WebApplication.TypeScript;
using JobTimer.WebApplication.ViewModels.WebApi.AdminUser.BindingModels;
using JobTimer.WebApplication.ViewModels.WebApi.AdminUser.Models;
using JobTimer.WebApplication.ViewModels.WebApi.AdminUser.ViewModels;
using JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.Role;
using JobTimer.WebApplication.ViewModels.WebApi.Common.Identity.User;
using Microsoft.AspNet.Identity;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class AdminUserController : BaseApiController
    {
        private readonly IUserAccess _userAccess;
        private readonly IUserCopier _userCopier;
        private readonly IRoleCopier _roleCopier;
        readonly IRequestReader _requestReader;

        public AdminUserController(IUserAccess userAccess, IUserCopier userCopier, IRoleCopier roleCopier, IRequestReader requestReader)
        {
            _userAccess = userAccess;
            _userCopier = userCopier;
            _roleCopier = roleCopier;
            _requestReader = requestReader;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetUsers(GetUsersBindingModel model)
        {
            var items = new GetUsersViewModel();

            var datas = await _userAccess.GetAllAsync(model.start, model.limit);
            var count = await _userAccess.CountAsync();

            foreach (var data in datas)
            {
                var d = new UserGridJson();

                d.ID = data.Id;
                d.UserName = data.UserName;
                d.Email = data.Email;

                items.data.results.Add(d);
            }
            items.data.hits = count;

            items.data.request.start = model.start;
            items.data.request.limit = model.limit;
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> ReadUser(ReadUserBindingModel model)
        {
            var result = new ReadUserViewModel();

            var data = await UserManager.FindByIdAsync(model.Data.ID);

            if (data != null)
            {
                var d = new UserJson();
                _userCopier.CopyUser(data, d);
                var roleids = data.Roles.Select(x => x.RoleId);
                d.Roles = await RoleManager.Roles.Where(x => roleids.Contains(x.Id)).Select(x => x.Name).ToListAsync();
                result.Data = d;
                result.Result = true;
            }
            else
                result.Result = false;

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> SaveUser(SaveUserBindingModel model)
        {
            var result = new SaveUserViewModel();

            IdentityResult identityResult = null;
            ApplicationUser user = null;
            if (string.IsNullOrEmpty(model.Data.Id))
            {
                user = new ApplicationUser() { UserName = model.Data.UserName, Email = model.Data.Email };

                identityResult = await UserManager.CreateAsync(user, model.Data.Password);
            }
            else
            {
                user = await UserManager.FindByIdAsync(model.Data.Id);

                user.UserName = model.Data.UserName;
                user.Email = model.Data.Email;

                identityResult = await UserManager.UpdateAsync(user);
            }

            if (!identityResult.Succeeded)
            {
                result.Result = false;
                if (identityResult.Errors != null)
                {
                    var firstError = identityResult.Errors.First();
                    result.Message = firstError;
                }
            }
            else
            {
                foreach (var roleId in user.Roles.Where(x => !model.Data.Roles.Contains(x.RoleId)).ToList())
                {
                    var role = await RoleManager.FindByIdAsync(roleId.RoleId);
                    if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                    {
                        var rolesResult = await UserManager.RemoveFromRoleAsync(user.Id, role.Name);

                        if (!rolesResult.Succeeded)
                        {
                            result.Result = false;
                            if (rolesResult.Errors != null)
                            {
                                var firstError = rolesResult.Errors.First();
                                result.Message = firstError;
                            }
                            break;
                        }
                    }
                }

                foreach (var role in model.Data.Roles)
                {
                    if (!await UserManager.IsInRoleAsync(user.Id, role))
                    {
                        var rolesResult = await UserManager.AddToRoleAsync(user.Id, role);

                        if (!rolesResult.Succeeded)
                        {
                            result.Result = false;
                            if (rolesResult.Errors != null)
                            {
                                var firstError = rolesResult.Errors.First();
                                result.Message = firstError;
                            }
                            break;
                        }
                    }
                }
                
                result.Result = true;
            }

            if (result.Result)
            {
                var notificationHub = GetHub<NotificationHub, INotificationHub>();

                var connectionId = _requestReader.Read(HttpHeaders.Request.SignalRConnectionId);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    notificationHub.Clients.AllExcept(connectionId).UpdateModel(new NotificationModel()
                    {
                        Username = UserName,
                        Action = string.Format("Ha salvato l'user {0}", model.Data.UserName)
                    });
                }
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> DeleteUser(DeleteUserBindingModel model)
        {
            var result = new DeleteUserViewModel();            
            result.Result = await _userAccess.DeleteAsync(model.Data.ID) > 0;

            if (result.Result)
            {
                var notificationHub = GetHub<NotificationHub, INotificationHub>();

                var connectionId = _requestReader.Read(HttpHeaders.Request.SignalRConnectionId);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    notificationHub.Clients.AllExcept(connectionId).UpdateModel(new NotificationModel()
                    {
                        Username = UserName,
                        Action = string.Format("Ha cancellato l'user {0}", model.Data.UserName)
                    });
                }
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetRoles()
        {
            var result = new GetRolesViewModel();
            var roles = await RoleManager.Roles.OrderBy(x => x.Id).ToListAsync();

            foreach (var role in roles)
            {
                var roleJson = new RoleSimpleJson();
                _roleCopier.CopyRole(role, roleJson);
                result.Roles.Add(roleJson);
            }

            result.Result = true;

            return Ok(result);
        }
    }
}