using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using JobTimer.Data.Model.JobTimer;

namespace JobTimer.Data.Access.JobTimer
{
    public interface IOvertimeAccess : IAccess<int, Overtime>
    {
        Task<Overtime> LoadAsync(string userName, DateTime date);
        Task<long?> SumAllOvertimesAsync(string userName);
    }
    public class OvertimeAccess : AccessWithInt<Overtime>, IOvertimeAccess
    {
        public OvertimeAccess(JobTimerDbContext context)
            : base(context)
        {
        }

        public async Task<Overtime> LoadAsync(string userName, DateTime date)
        {
            return await Set.Where(x => DbFunctions.TruncateTime(x.Date) == date.Date && x.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task<long?> SumAllOvertimesAsync(string userName)
        {
            return await Set.Where(x => x.UserName == userName).SumAsync(x => (long?)x.Time);
        }
    }
}