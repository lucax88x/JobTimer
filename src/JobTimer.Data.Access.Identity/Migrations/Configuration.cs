using JobTimer.Data.Model.Identity;
using JobTimer.Utils.Security;

namespace JobTimer.Data.Access.Identity.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<JobTimer.Data.Access.Identity.JIdentityDbContext>
    {
        private readonly IHashHelper _hashHelper;

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            _hashHelper = new HashHelper();
        }


        protected override void Seed(JobTimer.Data.Access.Identity.JIdentityDbContext context)
        {
            if (!context.Clients.Any())
            {
                context.Clients.Add(new Client
                {
                    Id = "jobTimer",
                    Secret = _hashHelper.Hash(SecretKeys.Clients.JobTimer),
                    Name = "JobTimer",                    
                    Active = true,                    
                    AllowedOrigin = "http://jobtime.azurewebsites.net"
                });

#if DEBUG
                context.Clients.Add(new Client
                {
                    Id = "jobTimerDebug",
                    Secret = _hashHelper.Hash(SecretKeys.Clients.JobTimerDebug),
                    Name = "JobTimer Debug",                    
                    Active = true,                    
                    AllowedOrigin = "*"
                });
#endif

                context.SaveChanges();
            }
        }
    }
}
