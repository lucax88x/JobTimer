using System;
using System.Web;

namespace JobTimer.WebApplication.Test
{
    public class SlowDownModule : IHttpModule
    {
        // In the Init function, register for HttpApplication 
        // events by adding your handlers.
        public void Init(HttpApplication application)
        {
            application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
            application.EndRequest += (new EventHandler(this.Application_EndRequest));
        }

        // Your BeginRequest event handler.
        private void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            var r = context.Request.RawUrl;

            if (r.Contains(".js") && (r.Contains("drag")))
            {
                System.Threading.Thread.Sleep(3000);
            }
        }

        // Your EndRequest event handler.
        private void Application_EndRequest(Object source, EventArgs e)
        {
        }

        public void Dispose()
        {
        }
    }
}
