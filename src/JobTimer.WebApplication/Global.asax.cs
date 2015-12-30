using System;
using System.Web;
using Common.Logging;


namespace JobTimer.WebApplication
{
    public class Global : HttpApplication
    {
        private ILog Log { get { return LogManager.GetLogger(this.GetType()); } }

        //LogManager.GetCurrentClassLogger().Info("Application Started");
        void Application_Start(object sender, EventArgs e)
        {
            Log.Info("Application Started");            
        }
        
        public void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();

            if (exc != null)
            {
                Log.Error("Application Error", exc);
            }
        }
    }
}