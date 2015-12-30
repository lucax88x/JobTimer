using System;

namespace JobTimer.Data.Model.JobTimer
{
    public class Overtime : Entity<int>
    {                
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public long Time { get; set; }
    }
}