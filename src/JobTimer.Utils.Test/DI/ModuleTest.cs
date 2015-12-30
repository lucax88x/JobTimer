using Autofac;
using FluentAssertions;
using NUnit.Framework;

namespace JobTimer.Utils.Test.DI
{
    public class ModuleTest
    {
        private IContainer _sut;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new JobTimer.Utils.DI.Module());
            _sut = builder.Build();
        }

        [Test]
        public void register_timegetter()
        {
            _sut.BeginLifetimeScope().Resolve<ITimeGetter>().Should().BeOfType<TimeGetter>();
        }

        [Test]
        public void register_timebuilder()
        {
            _sut.BeginLifetimeScope().Resolve<ITimeBuilder>().Should().BeOfType<TimeBuilder>();
        }
    }
}
