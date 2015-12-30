using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using JobTimer.Data.Access.JobTimer;
using JobTimer.Data.Dtos.Timer;
using JobTimer.Data.Model.JobTimer;
using JobTimer.Utils;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Timer = JobTimer.Data.Dtos.Timer.Timer;

namespace JobTimer.WebApplication.Logic.Test
{
    public class TimerCalculatorTest
    {
        private readonly IFixture _fixture = new Fixture();
        private ITimerCalculator _sut;
        private ITimerAccess _timerAccess;
        private ITimeBuilder _timeBuilder;

        [SetUp]
        public void SetUp()
        {
            _timerAccess = Substitute.For<ITimerAccess>();
            _timeBuilder = new TimeBuilder(new TimeGetter());
            _sut = new TimerCalculator(_timerAccess, _timeBuilder);
        }

        [Test]
        public async void calculateestimation_should_have_no_estimation()
        {
            var userName = _fixture.Create<string>();
            var enterExit = new EnterExit();

            _timerAccess.GetAllWithLunchOfToday(userName).Returns(enterExit);

            var result = await _sut.CalculateEstimation(userName);

            result.HasEstimatedExitTime.Should().BeFalse();
        }

        private readonly object[] _estimationTestCases = {
            new object[]
            {
                new List<TestData> {
                },
                false,
                null
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit)
                },
                false,
                null
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit),
                    new TestData("2005-11-01", "12:30", TimerTypes.Enter),
                    new TestData("2005-11-01", "15:30", TimerTypes.Exit),
                    new TestData("2005-11-01", "17:30", TimerTypes.Enter),
                    new TestData("2005-11-01", "20:54", TimerTypes.Exit)
                },
                false,
                null
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit),
                    new TestData("2005-11-01", "12:30", TimerTypes.Enter)
                },
                true,
                "18:54"
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit),
                    new TestData("2005-11-01", "12:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "13:30", TimerTypes.EnterLunch),
                    new TestData("2015-11-01", "14:30", TimerTypes.ExitLunch),
                    new TestData("2015-11-01", "16:30", TimerTypes.EnterLunch),
                    new TestData("2015-11-01", "16:40", TimerTypes.ExitLunch)

                },
                true,
                "20:04"
            }
        };

        [Test, TestCaseSource("_estimationTestCases")]
        public async void calculateestimation_should_have_correct_estimation(List<TestData> testDatas, bool hasEstimation, string estimatedExit)
        {
            var userName = _fixture.Create<string>();
            var enterExit = CreateEnterExitTimers(testDatas);

            _timerAccess.GetAllWithLunchOfToday(userName).Returns(enterExit);

            var result = await _sut.CalculateEstimation(userName);

            result.HasEstimatedExitTime.Should().Be(hasEstimation);

            if (!string.IsNullOrEmpty(estimatedExit))
            {
                result.EstimatedExitTime.TimeOfDay.ToString("hh':'mm").Should().Be(estimatedExit);
            }
        }

        private readonly object[] _totalsTestCases = {
            new object[]
            {
                new List<TestData> {
                },
                new List<string> {
                },
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit),
                    new TestData("2015-11-02", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-02", "12:00", TimerTypes.Exit)
                },
                new List<string> {
                    "02:00",
                    "03:30"
                },
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit),
                    new TestData("2015-11-02", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-02", "12:00", TimerTypes.Exit),
                    new TestData("2015-11-03", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-04", "6:00", TimerTypes.Enter),
                    new TestData("2015-11-04", "11:00", TimerTypes.EnterLunch),
                    new TestData("2015-11-04", "11:30", TimerTypes.ExitLunch),
                    new TestData("2015-11-04", "14:00", TimerTypes.Exit)
                },
                new List<string> {
                    "02:00",
                    "03:30",
                    "07:30"
                }
            }
        };

        [Test, TestCaseSource("_totalsTestCases")]
        public async void calculatetotals_should_have_correct_totals(List<TestData> testDatas, List<string> expectedDurations)
        {
            var userName = _fixture.Create<string>();
            var enterExit = CreateEnterExitTimers(testDatas);

            _timerAccess.GetAllWithLunchOfWeek(userName).Returns(enterExit);

            var result = await _sut.CalculateTotalsOfThisWeek(userName);

            result.Count.Should().Be(expectedDurations.Count);

            var i = 0;
            foreach (var kvp in result)
            {
                kvp.Value.ToString("hh':'mm").Should().BeEquivalentTo(expectedDurations[i]);
                i++;
            }
        }

        [Test, TestCaseSource("_totalsTestCases")]
        public async void calculatetotals_month_should_have_correct_totals(List<TestData> testDatas, List<string> expectedDurations)
        {
            var userName = _fixture.Create<string>();
            var enterExit = CreateEnterExitTimers(testDatas);

            _timerAccess.GetAllWithLunchOfMonth(userName).Returns(enterExit);

            var result = await _sut.CalculateTotalsOfThisMonth(userName);

            result.Count.Should().Be(expectedDurations.Count);

            var i = 0;
            foreach (var kvp in result)
            {
                kvp.Value.ToString("hh':'mm").Should().BeEquivalentTo(expectedDurations[i]);
                i++;
            }
        }

        private readonly object[] _totalsLunchTestCases = {
            new object[]
            {
                new List<TestData> {
                },
                new List<string> {
                },
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit),
                    new TestData("2015-11-02", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-02", "12:00", TimerTypes.Exit)
                },
                new List<string> {
                },
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "10:30", TimerTypes.Exit),
                    new TestData("2015-11-02", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-02", "12:00", TimerTypes.Exit),
                    new TestData("2015-11-03", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-04", "6:00", TimerTypes.Enter),
                    new TestData("2015-11-04", "11:00", TimerTypes.EnterLunch),
                    new TestData("2015-11-04", "11:30", TimerTypes.ExitLunch),
                    new TestData("2015-11-04", "14:00", TimerTypes.Exit)
                },
                new List<string> {
                    "00:30"
                }
            }
        };

        [Test, TestCaseSource("_totalsLunchTestCases")]
        public async void calculatetotals_lunch_should_have_correct_totals(List<TestData> testDatas, List<string> expectedDurations)
        {
            var userName = _fixture.Create<string>();
            var enterExit = CreateEnterExitTimers(testDatas);

            _timerAccess.GetAllOfLunchOfWeek(userName).Returns(enterExit);

            var result = await _sut.CalculateTotalsOfLunchOfThisWeek(userName);

            result.Count.Should().Be(expectedDurations.Count);

            var i = 0;
            foreach (var kvp in result)
            {
                kvp.Value.ToString("hh':'mm").Should().BeEquivalentTo(expectedDurations[i]);
                i++;
            }
        }

        [Test, TestCaseSource("_totalsLunchTestCases")]
        public async void calculatetotals_lunch_month_should_have_correct_totals(List<TestData> testDatas, List<string> expectedDurations)
        {
            var userName = _fixture.Create<string>();
            var enterExit = CreateEnterExitTimers(testDatas);

            _timerAccess.GetAllOfLunchOfMonth(userName).Returns(enterExit);

            var result = await _sut.CalculateTotalsOfLunchOfThisMonth(userName);

            result.Count.Should().Be(expectedDurations.Count);

            var i = 0;
            foreach (var kvp in result)
            {
                kvp.Value.ToString("hh':'mm").Should().BeEquivalentTo(expectedDurations[i]);
                i++;
            }
        }

        private static EnterExit CreateEnterExitTimers(List<TestData> testDatas)
        {
            var enterExit = new EnterExit();

            foreach (var testData in testDatas)
            {
                var timer = CreateTimer(testData);
                switch (testData.Type)
                {
                    case TimerTypes.Enter:
                        enterExit.EnterTimers.Add(timer);
                        break;
                    case TimerTypes.EnterLunch:
                        enterExit.EnterLunchTimers.Add(timer);
                        break;
                    case TimerTypes.Exit:
                        enterExit.ExitTimers.Add(timer);
                        break;
                    case TimerTypes.ExitLunch:
                        enterExit.ExitLunchTimers.Add(timer);
                        break;
                }
            }
            return enterExit;
        }

        private readonly object[] _overtimesTestCases = {
            new object[]
            {
                new List<TestData> {
                },
                "00:00"
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "16:54", TimerTypes.Exit)
                },
                "00:00"
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "12:30", TimerTypes.EnterLunch),
                    new TestData("2015-11-01", "13:30", TimerTypes.ExitLunch),
                    new TestData("2015-11-01", "16:54", TimerTypes.Exit)
                },
                "-01:00"
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "19:00", TimerTypes.Exit)
                },
                "+02:06"
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "12:30", TimerTypes.EnterLunch),
                    new TestData("2015-11-01", "13:00", TimerTypes.ExitLunch),
                    new TestData("2015-11-01", "18:54", TimerTypes.Exit)
                },
                "+01:30"
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-01", "8:30", TimerTypes.Enter),
                    new TestData("2015-11-01", "12:30", TimerTypes.EnterLunch),
                    new TestData("2015-11-01", "13:00", TimerTypes.ExitLunch),
                    new TestData("2015-11-01", "13:54", TimerTypes.Exit)
                },
                "-03:30"
            },
            new object[]
            {
                new List<TestData> {
                    new TestData("2015-11-12", "08:00", TimerTypes.Enter),
                    new TestData("2015-11-12", "10:30", TimerTypes.EnterLunch),
                    new TestData("2015-11-12", "12:00", TimerTypes.ExitLunch),
                    new TestData("2015-11-12", "16:15", TimerTypes.Exit)
                },
                "-01:39"
            }
        };

        [Test, TestCaseSource("_overtimesTestCases")]
        public async void calculateovertimes_should_have_correct_overtime(List<TestData> testDatas, string expectedOvertime)
        {
            var userName = _fixture.Create<string>();
            var enterExit = CreateEnterExitTimers(testDatas);

            var date = _fixture.Create<DateTime>();

            _timerAccess.GetAllWithLunchOfDate(userName, date).Returns(enterExit);

            var result = await _sut.CalculateOvertime(userName, date);

            var operand = "";
            if (result.TotalMilliseconds > 0)
                operand = "+";
            else if (result.TotalMilliseconds < 0)
                operand = "-";

            (operand + result.ToString("hh':'mm")).Should().Be(expectedOvertime);
        }

        private static Timer CreateTimer(TestData testData)
        {
            return new Timer()
            {
                Date = DateTime.ParseExact(testData.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Time = TimeSpan.Parse(testData.Time),
                TimerType = (int)testData.Type
            };
        }

        public class TestData
        {
            public readonly string Date;
            public readonly string Time;
            public readonly TimerTypes Type;

            public TestData(string date, string time, TimerTypes type)
            {
                Date = date;
                Time = time;
                Type = type;
            }
        }
    }
}


