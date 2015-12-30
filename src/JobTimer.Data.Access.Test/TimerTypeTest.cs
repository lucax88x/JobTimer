using FluentAssertions;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace JobTimer.Data.Access.Test
{
    [TestFixture]
    public class TimerTypeTest
    {

        Fixture _fixture;
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public async void timerType_insert_and_delete()
        {
            var context = new JobTimerDbContext();
            var timerTypeAccess = new TimerTypeAccess(context);

            var timerType = new TimerType { ID = 99999, Type = _fixture.Create<string>() };

            await timerTypeAccess.SaveAsync(timerType);

            timerType.ID.Should().NotBe(0);

            var timerTypeLoaded = await timerTypeAccess.LoadAsync(timerType.ID);

            timerTypeLoaded.Type.Should().Be(timerType.Type);

            var deletedCount = await timerTypeAccess.DeleteAsync(timerTypeLoaded);

            deletedCount.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public async void timerTypes_get_at_least_one()
        {
            var context = new JobTimerDbContext();
            var timerTypeAccess = new TimerTypeAccess(context);
            var hrs = await timerTypeAccess.GetAllAsync();
            hrs.Count.Should().BeGreaterThan(0);
        }
    }
}

