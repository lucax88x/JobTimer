using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobTimer.Data.Model.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JobTimer.Data.Access.Identity
{
    public interface IAuthAccess
    {
        Task<IdentityResult> RegisterUser(string userName, string password);
        Task<ApplicationUser> FindUser(string userName, string password);
        Client FindClient(string clientId);        
        Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password = "");
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
    }

    public class AuthAccess : IAuthAccess, IDisposable
    {
        private readonly JIdentityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthAccess(JIdentityDbContext context)
        {
            _context = context;
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
        }

        public async Task<IdentityResult> RegisterUser(string userName, string password)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userName
            };

            var result = await _userManager.CreateAsync(user, password);

            return result;
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public Client FindClient(string clientId)
        {
            var client = _context.Clients.Find(clientId);

            return client;
        }
                
        public async Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {
            ApplicationUser user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password = "")
        {
            if (!string.IsNullOrEmpty(password))
            {
                return await _userManager.CreateAsync(user, password);
            }
            else
            {
                return await _userManager.CreateAsync(user);
            }
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return _userManager.AddToRoleAsync(user.Id, role);
        }

        public void Dispose()
        {
            _context.Dispose();
            _userManager.Dispose();
        }
    }
}
