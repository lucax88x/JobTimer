using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace JobTimer.Data.Model.JobTimer.Mappings
{
    public class TimerMapping : EntityTypeConfiguration<Timer>
    {
        public TimerMapping()
        {
            ToTable("Timer");

            HasKey(x => x.ID);

            Property(x => x.ID)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.UserName).HasColumnName("UserName");
            Property(x => x.TimerTypeId).HasColumnName("Type");
            Property(x => x.Date).HasColumnName("Date");
            Property(x => x.Time).HasColumnName("Time");

        }
    }
}
