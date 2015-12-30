using System;
using System.Collections.Generic;

namespace JobTimer.Data.Dtos.Timer
{
    public class EnterExitRanges
    {
        public List<Timer> EnterTimers { get; set; }
        public List<Timer> ExitTimers { get; set; }

        public EnterExitRanges()
        {
            EnterTimers = new List<Timer>();
            ExitTimers = new List<Timer>();
        }
    }

    public class EnterExit
    {
        public List<Timer> EnterTimers { get; set; }
        public List<Timer> EnterLunchTimers { get; set; }
        public List<Timer> ExitTimers { get; set; }
        public List<Timer> ExitLunchTimers { get; set; }

        public EnterExit()
        {
            EnterTimers = new List<Timer>();
            EnterLunchTimers = new List<Timer>();
            ExitTimers = new List<Timer>();            
            ExitLunchTimers = new List<Timer>();
        }
    }
}
