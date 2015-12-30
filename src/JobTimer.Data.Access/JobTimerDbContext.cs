using System.Data.Entity;
using System.Diagnostics;
using Common.Logging;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Data.Model.JobTimer.Mappings;

namespace JobTimer.Data.Access
{
    public class JobTimerDbContext : DbContext
    {
        private ILog Log { get { return LogManager.GetLogger(this.GetType()); } }

        public JobTimerDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new JobTimerInitializer());

            this.Database.Log = (query) =>
            {

#if DEBUG
                Debug.WriteLine(query);
#endif
                Log.Info(query);
            };
        }

        public DbSet<Shortcut> Shortcuts { get; set; }
        public DbSet<LastVisited> LastVisiteds { get; set; }
        public DbSet<TimerType> TimerType { get; set; }
        public DbSet<Timer> Timer { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ShortcutMapping());
            modelBuilder.Configurations.Add(new LastVisitedMapping());
            modelBuilder.Configurations.Add(new TimerTypeMapping());
            modelBuilder.Configurations.Add(new TimerMapping());
            modelBuilder.Configurations.Add(new OvertimeMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
