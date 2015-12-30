using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Integration.WebApi;
using JobTimer.WebApplication;
using JobTimer.WebApplication.Hubs.Pipelines;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace JobTimer.WebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //var config = GlobalConfiguration.Configuration;
            var config = new HttpConfiguration();

            ConfigureAuth(app);            

            var container = AutofacConfig.Config(config);

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(config);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.Register(container);            

            DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(container));
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);            
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            SignalrConfig.Config(app);
            app.UseWebApi(config);
        }
    }
}
