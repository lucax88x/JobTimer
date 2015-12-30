using JobTimer.Data.Model.JobTimer;

namespace JobTimer.Data.Access.JobTimer
{
    public interface ITimerTypeAccess : IAccess<int, TimerType>
    {
    }
    public class TimerTypeAccess : AccessWithInt<TimerType>, ITimerTypeAccess
    {
        public TimerTypeAccess(JobTimerDbContext context)
            : base(context)
        {
        }
    }
}
