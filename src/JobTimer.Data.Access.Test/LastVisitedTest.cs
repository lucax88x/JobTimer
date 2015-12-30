using FluentAssertions;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace JobTimer.Data.Access.Test
{
    [TestFixture]
    public class LastVisitedTest
    {
        Fixture _fixture;
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async void lastvisited_insert_and_delete()
        {
            var context = new JobTimerDbContext();
            var lastVisitedAccess = new LastVisitedAccess(context);

            var lastVisited = new LastVisited();
            lastVisited.UserName = _fixture.Create<string>();
            lastVisited.Visited = _fixture.Create<string>();

            await lastVisitedAccess.SaveAsync(lastVisited);

            lastVisited.ID.Should().NotBe(0);
            
            var loaded = await lastVisitedAccess.LoadAsync(lastVisited.ID);

            loaded.UserName.Should().Be(lastVisited.UserName);
            loaded.Visited.Should().Be(lastVisited.Visited);

            var deletedCount = await lastVisitedAccess.DeleteAsync(loaded);

            deletedCount.Should().BeGreaterOrEqualTo(1);
        }
    }
}

