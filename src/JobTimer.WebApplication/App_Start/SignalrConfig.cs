using JobTimer.WebApplication.Hubs.Pipelines;
using Microsoft.AspNet.SignalR;
using Owin;

namespace JobTimer.WebApplication
{
    public static class SignalrConfig
    {
        public static void Config(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration { EnableJavaScriptProxies = true };
#if DEBUG
            hubConfiguration.EnableDetailedErrors = true;
#endif
            app.MapSignalR("/signalr", hubConfiguration);
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
        }
    }
}