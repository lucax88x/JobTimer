using System.Net.Http;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FluentAssertions;
using NUnit.Framework;

namespace JobTimer.WebApplication.Logic.Test.DI
{
    public class ModuleTest
    {
        private HttpConfiguration _configuration = null;
        private ILifetimeScope _sut;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new JobTimer.WebApplication.Logic.DI.Module());
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
        public void register_ITimerCalculator()
        {
            _sut.Resolve<ITimerCalculator>().Should().BeOfType<TimerCalculator>();
        }
    }
}
