using System;

namespace JobTimer.WebApplication.Hubs.Models.Notification
{
    public class NotificationModel
    {
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }

        public NotificationModel()
        {
            Date = DateTime.Now;
        }
    }
}
