using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using JobTimer.Data.Access;
using JobTimer.Data.Access.DI;
using JobTimer.Data.Access.Identity;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Utils.DI;
using JobTimer.WebApplication.ActionFilters;
using JobTimer.WebApplication.Code;
using JobTimer.WebApplication.CopyPresets.Identity;
using JobTimer.WebApplication.Initializers;
using JobTimer.WebApplication.Loaders.Master;
using JobTimer.WebApplication.Loaders.Navigation;
using Microsoft.AspNet.SignalR;

namespace JobTimer.WebApplication
{
    public static class AutofacConfig
    {
        public static IContainer Config(HttpConfiguration config)
        {
#if DEBUG
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
#endif

            var builder = new ContainerBuilder();

            builder.RegisterModule(new Utils.DI.Module());

            builder.RegisterType<UserCopier>().As<IUserCopier>();
            builder.RegisterType<RoleCopier>().As<IRoleCopier>();
            
            builder.RegisterModule(new JobTimer.Data.Access.DI.Module());
            builder.RegisterModule(new JobTimer.Data.Access.Identity.DI.Module());
            builder.RegisterModule(new JobTimer.WebApplication.Loaders.DI.Module());
            builder.RegisterModule(new JobTimer.WebApplication.Logic.DI.Module());

            #region webapi

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterType<ServerValidationAttribute>().AsWebApiActionFilterFor<ApiController>().InstancePerRequest();
            builder.RegisterType<ExceptionAttribute>().AsWebApiExceptionFilterFor<ApiController>().InstancePerRequest();

            #endregion

            #region signalr

            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            #endregion

            #region code

            builder.RegisterType<RequestReader>().As<IRequestReader>().InstancePerRequest();

            #endregion

            Database.SetInitializer(new IdentityDbInitializer());
            var identityContext = new JIdentityDbContext();
            identityContext.Database.Initialize(true);

            Database.SetInitializer(new NullDatabaseInitializer<JobTimerDbContext>());
            var jobTimer = new JobTimerDbContext();
            jobTimer.Database.Initialize(true);

            var container = builder.Build();
            return container;
        }

    }
}