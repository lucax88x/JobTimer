using Common.Logging;
using Microsoft.AspNet.SignalR.Hubs;

namespace JobTimer.WebApplication.Hubs.Pipelines
{
    public class ErrorHandlingPipelineModule : HubPipelineModule
    {
        private ILog _logger = LogManager.GetLogger(typeof(ErrorHandlingPipelineModule));

        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            _logger.Error(exceptionContext.Error);
            
            base.OnIncomingError(exceptionContext, invokerContext);

        }
    }
}
