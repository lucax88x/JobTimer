using System.Web.Mvc;
using System.Web.Routing;

namespace JobTimer.WebApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Timer",
                url: "timer",
                defaults: new { controller = "Index", action = "Timer"}
            );

            routes.MapMvcAttributeRoutes();
 
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Index", action = "Index", id = UrlParameter.Optional }
            );            
        }
    }
}
