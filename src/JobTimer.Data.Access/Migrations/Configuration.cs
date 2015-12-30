using JobTimer.Data.Model.JobTimer;

namespace JobTimer.Data.Access.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<JobTimerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(JobTimerDbContext context)
        {
            if (!context.TimerType.Any())
            {
                context.TimerType.Add(new TimerType() { ID = 1, Type = "Enter" });
                context.TimerType.Add(new TimerType() { ID = 2, Type = "Exit" });
                context.TimerType.Add(new TimerType() { ID = 3, Type = "EnterLunch" });
                context.TimerType.Add(new TimerType() { ID = 4, Type = "ExitLunch" });
                context.SaveChanges();
            }
        }
    }
}
