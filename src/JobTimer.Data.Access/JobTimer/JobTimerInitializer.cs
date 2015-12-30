using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobTimer.Data.Model.JobTimer;

namespace JobTimer.Data.Access.JobTimer
{
    public class JobTimerInitializer : DropCreateDatabaseIfModelChanges<JobTimerDbContext>
    {
        protected override void Seed(JobTimerDbContext context)
        {
            foreach (var timerType in Enum.GetValues(typeof(TimerTypes)))
            {
                context.TimerType.Add(new TimerType() { ID = (int)timerType, Type = timerType.ToString() });
            }

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
