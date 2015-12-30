using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;

namespace JobTimer.WebApplication.ActionFilters
{
    public class ServerValidationAttribute : IAutofacActionFilter
    {
        public void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var firstModelError = actionContext.ModelState.First();
                var firstError = firstModelError.Value.Errors.First();

                var msg = string.Empty;
                if (!string.IsNullOrEmpty(firstError.ErrorMessage))
                {
                    msg = firstError.ErrorMessage;
                }
                else
                {
#if DEBUG
                    if (firstError.Exception != null)
                    {
                        if (!string.IsNullOrEmpty(firstError.Exception.Message))
                        {
                            msg = firstError.Exception.Message;
                        }
                    }
#endif
                }

                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, msg);
            }

        }

        public void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            
        }
    }
}