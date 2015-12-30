using Autofac;
using Microsoft.AspNet.SignalR;

namespace JobTimer.WebApplication.Hubs
{
    public class BaseHub<T> : Hub<T> where T: class
    {
        private readonly ILifetimeScope _hubLifetimeScope;
        public ILifetimeScope Scope => _hubLifetimeScope;

        public BaseHub(ILifetimeScope lifetimeScope)
        {
            // Create a lifetime scope for the hub.
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();

            // Resolve dependencies from the hub lifetime scope.
            //_logger = _hubLifetimeScope.Resolve<ILogger>();
        }

        protected override void Dispose(bool disposing)
        {
            // Dipose the hub lifetime scope when the hub is disposed.
            if (disposing && _hubLifetimeScope != null)
            {
                _hubLifetimeScope.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
