using System.Net.Http;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FluentAssertions;
using NUnit.Framework;

namespace JobTimer.Data.Access.Identity.Test.DI
{
    public class ModuleTest
    {
        private HttpConfiguration _configuration = null;
        private ILifetimeScope _sut;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new JobTimer.Data.Access.Identity.DI.Module());
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
        public void resolve_IdentityDbContext()
        {
            _sut.Resolve<JIdentityDbContext>().Should().BeOfType<JIdentityDbContext>();
        }

        [Test]
        public void resolve_IUserAccess()
        {
            _sut.BeginLifetimeScope().Resolve<IUserAccess>().Should().BeOfType<UserAccess>();
        }

        [Test]
        public void resolve_IAuthAccess()
        {
            _sut.BeginLifetimeScope().Resolve<IAuthAccess>().Should().BeOfType<AuthAccess>();
        }

    }
}
