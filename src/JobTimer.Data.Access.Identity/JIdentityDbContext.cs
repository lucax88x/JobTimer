using System.Data.Entity;
using JobTimer.Data.Model.Identity;
using JobTimer.Data.Model.Identity.Mappings;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JobTimer.Data.Access.Identity
{
    public class JIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public JIdentityDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static JIdentityDbContext Create()
        {
            return new JIdentityDbContext();
        }

        public DbSet<Client> Clients { get; set; }        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ClientMapping());            

            base.OnModelCreating(modelBuilder);
        }
    }
}