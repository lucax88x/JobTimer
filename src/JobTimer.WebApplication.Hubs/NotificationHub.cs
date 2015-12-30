using Autofac;
using JobTimer.WebApplication.Hubs.Models.Notification;

namespace JobTimer.WebApplication.Hubs
{
    public interface INotificationHub
    {
        void Starter();
        void UpdateModel(NotificationModel model);
    }

    public class NotificationHub : BaseHub<INotificationHub>
    {
        public NotificationHub(ILifetimeScope lifetimeScope) : base(lifetimeScope)
        {
        }

        public void Starter()
        {
            Clients.All.Starter();
        }
        public void UpdateModel(NotificationModel notificationModel)
        {
            Clients.All.UpdateModel(notificationModel);
        }
    }
}