using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace JobTimer.Data.Model.JobTimer.Mappings
{
    public class ShortcutMapping: EntityTypeConfiguration<Shortcut>
    {
        public ShortcutMapping()
        {
            ToTable("Shortcuts");

            HasKey(x => x.ID);

            Property(x => x.ID)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.UserName).HasColumnName("UserName");
            Property(x => x.Shortcuts).HasColumnName("Shortcuts");
        }
    }
}
