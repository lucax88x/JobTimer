using System.Collections.Generic;
using System.Data.Entity;
using JobTimer.Data.Access.Identity;
using JobTimer.Data.Model.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JobTimer.WebApplication.Initializers
{
    public class IdentityDbInitializer : DropCreateDatabaseIfModelChanges<JIdentityDbContext>
    {
        public override void InitializeDatabase(JIdentityDbContext context)
        {
            //base.InitializeDatabase(context);

            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            CreateRoles(roleManager);
            CreateAdmin(userManager, roleManager);            
        }

        private void CreateRoles(ApplicationRoleManager roleManager)
        {
            var roles = new List<string> { "TimerUser" };
            foreach (var roleName in roles)
            {                
                var role = roleManager.FindByName(roleName);
                if (role == null)
                {
                    role = new IdentityRole(roleName);
                    roleManager.Create(role);
                }
            }
        }
        private void CreateAdmin(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            const string adminUser = "lucax88x@gmail.com";
            const string adminEmail = "lucax88x@gmail.com";
            const string roleName = "Admin";
            
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                roleManager.Create(role);
            }

            var user = userManager.FindByName(adminUser);
            if (user == null)
            {
                user = new ApplicationUser { UserName = adminUser, Email = adminEmail };
                var result = userManager.Create(user, SecretKeys.Passwords.Admin);
                userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }
}