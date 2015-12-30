using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;

namespace JobTimer.WebApplication.ActionFilters
{
    public class ExceptionAttribute : IAutofacExceptionFilter
    {
        public void OnException(HttpActionExecutedContext actionExecutedContext)
        {
#if DEBUG
            if (actionExecutedContext.Exception != null)
            {
                if (actionExecutedContext.Exception.InnerException != null)
                {
                    if (actionExecutedContext.Exception.InnerException.InnerException != null)
                    {
                        actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, actionExecutedContext.Exception.InnerException.InnerException.Message);
                    }
                    else
                    {
                        actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, actionExecutedContext.Exception.InnerException.Message);
                    }
                }
                else
                {
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, actionExecutedContext.Exception.Message);
                }
            }
#else
            actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Internal Server Error");
#endif

        }
    }
}