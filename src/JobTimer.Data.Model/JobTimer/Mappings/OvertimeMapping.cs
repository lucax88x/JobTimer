using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace JobTimer.Data.Model.JobTimer.Mappings
{
    public class OvertimeMapping : EntityTypeConfiguration<Overtime>
    {
        public OvertimeMapping()
        {
            ToTable("Overtime");

            HasKey(x => x.ID);

            Property(x => x.ID)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.UserName).HasColumnName("UserName");            
            Property(x => x.Date).HasColumnName("Date").HasColumnType("date");
            Property(x => x.Time).HasColumnName("Time").HasColumnType("bigint");
        }
    }
}