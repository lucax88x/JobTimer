namespace JobTimer.Data.Model.JobTimer
{
    public class LastVisited : Entity<int>
    {
        public string UserName { get; set; }
        public string Visited { get; set; }
    }
}
