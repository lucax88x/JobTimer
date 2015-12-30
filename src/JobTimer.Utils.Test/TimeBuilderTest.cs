using System;
using Autofac;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace JobTimer.Utils.Test
{
    public class TimeBuilderTest
    {
        private ITimeBuilder _sut;
        private ITimeGetter _timeGetter;

        [SetUp]
        public void Setup()
        {
            _timeGetter = Substitute.For<ITimeGetter>();
            _sut = new TimeBuilder(_timeGetter);
        }
        
        [TestCase("2015-11-11", "2015-11-09")]
        [TestCase("2014-05-12", "2014-05-12")]
        [TestCase("2012-01-29", "2012-01-23")]
        [TestCase("2016-09-20", "2016-09-19")]
        [TestCase("2004-07-15", "2004-07-12")]
        [TestCase("2024-04-05", "2024-04-01")]
        public void build_start_of_week(string date, string startDate)
        {
            _timeGetter.Now.Returns(DateTime.Parse(date));
            _sut.GetWeekStartDelimiter().ToString("yyyy-MM-dd").Should().Be(startDate);
        }

        [TestCase("2015-08-31", "2015-09-06")]
        [TestCase("1993-04-29", "1993-05-02")]
        public void build_end_of_week(string date, string endDate)
        {
            _timeGetter.Now.Returns(DateTime.Parse(date));
            _sut.GetWeekEndDelimiter().ToString("yyyy-MM-dd").Should().Be(endDate);
        }

        [TestCase("2005-11-01", "2005-11-01")]
        [TestCase("2015-11-11", "2015-11-01")]
        [TestCase("2014-05-12", "2014-05-01")]
        [TestCase("2012-01-29", "2012-01-01")]
        [TestCase("2016-09-20", "2016-09-01")]
        [TestCase("2004-07-15", "2004-07-01")]
        [TestCase("2024-04-05", "2024-04-01")]
        public void build_start_of_month(string date, string startDate)
        {
            _timeGetter.Now.Returns(DateTime.Parse(date));
            _sut.GetMonthStartDelimiter().ToString("yyyy-MM-dd").Should().Be(startDate);
        }

        [TestCase("2015-08-31", "2015-08-31")]
        [TestCase("1993-04-29", "1993-04-30")]
        [TestCase("2014-05-12", "2014-05-31")]
        [TestCase("1956-02-12", "1956-02-29")]
        [TestCase("2012-01-29", "2012-01-31")]
        [TestCase("2016-09-20", "2016-09-30")]
        [TestCase("2004-07-15", "2004-07-31")]
        [TestCase("2024-04-05", "2024-04-30")]
        public void build_end_of_month(string date, string endDate)
        {
            _timeGetter.Now.Returns(DateTime.Parse(date));
            _sut.GetMonthEndDelimiter().ToString("yyyy-MM-dd").Should().Be(endDate);
        }


        [TestCase("2015-11-11")]
        [TestCase("2014-05-12")]
        [TestCase("2012-01-29")]
        [TestCase("2016-09-20")]
        [TestCase("2004-07-15")]
        [TestCase("2024-04-05")]
        [TestCase("2015-08-31")]
        [TestCase("1993-04-29")]
        public void get_week(string date)
        {
            _timeGetter.Now.Returns(DateTime.Parse(date));
            var result = _sut.GetWeek();

            result.Should().HaveCount(7);

            result[0].ToString("yyyy-M-d dddd").Should().Be(_sut.GetWeekStartDelimiter().ToString("yyyy-M-d dddd"));
            result[6].ToString("yyyy-M-d dddd").Should().Be(_sut.GetWeekEndDelimiter().ToString("yyyy-M-d dddd"));
        }
    }
}
