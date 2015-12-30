using Autofac;

namespace JobTimer.Data.Access.Identity.DI
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new Utils.DI.Module());

            builder.RegisterType<JIdentityDbContext>().As<JIdentityDbContext>().InstancePerRequest();
            builder.RegisterType<UserAccess>().As<IUserAccess>().InstancePerRequest();
            builder.RegisterType<AuthAccess>().As<IAuthAccess>().InstancePerRequest();          
        }
    }
}
