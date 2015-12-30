using System.Web.Http.ExceptionHandling;
using Common.Logging;

namespace JobTimer.WebApplication
{
    public class LogExceptionLogger : ExceptionLogger
    {
        private ILog _logger = LogManager.GetLogger(typeof(LogExceptionLogger));

        public override void Log(ExceptionLoggerContext context)
        {
            if (context.Exception != null)
            {
                _logger.Error(context.Exception);                
            }
        }
    }
}