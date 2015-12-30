using System;

namespace JobTimer.Data.Dtos.Timer
{
    public class Timer
    {
        public int TimerType { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
