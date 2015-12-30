using Autofac;
using JobTimer.WebApplication.Loaders.Master;
using JobTimer.WebApplication.Loaders.Navigation;

namespace JobTimer.WebApplication.Loaders.DI
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MenuLoader>().As<IMenuLoader>().SingleInstance();
            builder.RegisterType<BundlesLoader>().As<IBundlesLoader>().SingleInstance();
        }
    }
}
