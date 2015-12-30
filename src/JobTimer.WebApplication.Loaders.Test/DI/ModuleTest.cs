using Autofac;
using FluentAssertions;
using JobTimer.WebApplication.Loaders.Master;
using JobTimer.WebApplication.Loaders.Navigation;
using NUnit.Framework;

namespace JobTimer.WebApplication.Loaders.Test.DI
{
    public class ModuleTest
    {
        private IContainer _sut;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new JobTimer.WebApplication.Loaders.DI.Module());
            _sut = builder.Build();
        }

        [Test]
        public void resolve_IMenuLoader()
        {
            _sut.BeginLifetimeScope().Resolve<IMenuLoader>().Should().BeOfType<MenuLoader>();
        }

        [Test]
        public void resolve_IBundlesLoader()
        {
            _sut.BeginLifetimeScope().Resolve<IBundlesLoader>().Should().BeOfType<BundlesLoader>();
        }
    }
}
