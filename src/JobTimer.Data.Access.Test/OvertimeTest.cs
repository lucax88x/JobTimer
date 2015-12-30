using System;
using FluentAssertions;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Model.JobTimer;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace JobTimer.Data.Access.Test
{
    [TestFixture]
    public class OvertimeTest
    {
        private JobTimerDbContext _context;
        Fixture _fixture;
        private OvertimeAccess _sut;        
        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();            
            _context = new JobTimerDbContext();
            _sut = new OvertimeAccess(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async void overtime_insert_and_delete()
        {
            var item = new Overtime
            {                
                UserName = _fixture.Create<string>(),
                Date = _fixture.Create<DateTime>(),
                Time = _fixture.Create<long>()
            };

            await _sut.SaveAsync(item);

            item.ID.Should().NotBe(0);

            var loaded = await _sut.LoadAsync(item.ID);            

            loaded.UserName.Should().Be(item.UserName);
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