using System;
namespace JobTimer.Data.Model.JobTimer
{
    public class Timer : Entity<int>
    {
        public virtual TimerType TimerType { get; set; }
        public int TimerTypeId { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
