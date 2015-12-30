using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace JobTimer.Data.Model.Identity.Mappings
{
    public class ClientMapping : EntityTypeConfiguration<Client>
    {
        public ClientMapping()
        {
            ToTable("Clients");

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("Id");

            Property(x => x.Secret).HasColumnName("Secret").IsRequired();
            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);

            Property(x => x.Active).HasColumnName("Active");           
            Property(x => x.AllowedOrigin).HasColumnName("AllowedOrigin").HasMaxLength(100);
        }
    }
}
