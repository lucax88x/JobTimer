using System;
using System.Linq;
using FluentAssertions;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Utils;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace JobTimer.Data.Access.Test
{
    [TestFixture]
    public class TimerTest
    {
        private JobTimerDbContext _context;
        Fixture _fixture;
        private TimerAccess _sut;
        private ITimeBuilder _timeBuilder;
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _timeBuilder = Substitute.For<ITimeBuilder>();
            _context = new JobTimerDbContext();
            _sut = new TimerAccess(_context, _timeBuilder);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async void timer_insert_and_delete()
        {
            var timerTypeAccess = new TimerTypeAccess(_context);

            var item = new Timer
            {
                TimerTypeId = 2,
                UserName = _fixture.Create<string>(),
                Date = _fixture.Create<DateTime>(),
                Time = _fixture.Create<TimeSpan>()
            };

            await _sut.SaveAsync(item);

            item.ID.Should().NotBe(0);

            var loaded = await _sut.LoadAsync(item.ID);
            var typeLoaded = await timerTypeAccess.LoadAsync(item.TimerTypeId);

            loaded.UserName.Should().Be(item.UserName);
            loaded.TimerType.Type.Should().Be(typeLoaded.Type);
            loaded.Date.Should().Be(item.Date);
            loaded.Time.Should().Be(item.Time);

            var deletedCount = await _sut.DeleteAsync(loaded);

            deletedCount.Should().BeGreaterOrEqualTo(1);
        }
        //[Test]
        //public async void timer_get_at_least_one()
        //{        
        //    var hrs = await timerAccess.GetAllAsync();
        //    hrs.Count.Should().BeGreaterThan(0);
        //}        
    }
}

