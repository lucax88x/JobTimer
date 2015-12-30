using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using FluentAssertions;
using JobTimer.Data.Access.JobTimer;
using NUnit.Framework;

namespace JobTimer.Data.Access.Test.DI
{
    public class ModuleTest
    {
        private HttpConfiguration _configuration = null;
        private ILifetimeScope _sut;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {            
            var builder = new ContainerBuilder();
            builder.RegisterModule(new Data.Access.DI.Module());
            var container = builder.Build();

            this._configuration = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(container)
            };

            var message = new HttpRequestMessage();
            message.SetConfiguration(this._configuration);            
            _sut = message.GetDependencyScope().GetRequestLifetimeScope();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {            
            this._configuration.Dispose();
        }
                
        [Test]
        public void resolve_JobTimerDbContext()
        {
            _sut.BeginLifetimeScope().Resolve<JobTimerDbContext>().Should().BeOfType<JobTimerDbContext>();
        }

        [Test]
        public void resolve_IShortcutAccess()
        {
            _sut.BeginLifetimeScope().Resolve<IShortcutAccess>().Should().BeOfType<ShortcutAccess>();
        }

        [Test]
        public void resolve_ILastVisitedAccess()
        {
            _sut.BeginLifetimeScope().Resolve<ILastVisitedAccess>().Should().BeOfType<LastVisitedAccess>();
        }

        [Test]
        public void resolve_ITimerAccess()
        {
            _sut.BeginLifetimeScope().Resolve<ITimerAccess>().Should().BeOfType<TimerAccess>();
        }

        [Test]
        public void resolve_ITimerTypeAccess()
        {
            _sut.BeginLifetimeScope().Resolve<ITimerTypeAccess>().Should().BeOfType<TimerTypeAccess>();
        }
    }
}
