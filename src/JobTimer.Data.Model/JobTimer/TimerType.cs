namespace JobTimer.Data.Model.JobTimer
{
    public enum TimerTypes
    {
        Enter = 1,
        Exit = 2,
        EnterLunch = 3,
        ExitLunch = 4
    }
    public class TimerType: Entity<int>
    {        
        public string Type { get; set; }
    }
}
