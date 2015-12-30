using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace JobTimer.Data.Model.JobTimer.Mappings
{
    public class TimerTypeMapping: EntityTypeConfiguration<TimerType>
    {
        public TimerTypeMapping()
        {
            ToTable("TimerType");

            HasKey(x => x.ID);

            Property(x => x.ID)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Type).HasColumnName("Type");
        }
    }
}
