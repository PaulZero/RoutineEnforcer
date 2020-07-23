using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Tests.Data.Models.Serialisation;
using System;

namespace PaulZero.RoutineEnforcer.Tests.Models
{
    [TestClass]
    public class NoComputerPeriodTest
    {
        [TestMethod]
        public void CanCorrectlyDeterminePeriodIsActiveAcrossDays()
        {
            var period = new NoComputerPeriod
            {
                StartTime = new TimeSpan(23, 0, 0),
                EndTime = new TimeSpan(1, 0, 0),
                DaysActive = DaySelection.Daily
            };

            Assert.IsFalse(period.IsActiveAt(new DateTime(2020, 7, 23, 22, 59, 59)));
            Assert.IsTrue(period.IsActiveAt(new DateTime(2020, 7, 23, 23, 0, 0)));
            Assert.IsTrue(period.IsActiveAt(new DateTime(2020, 7, 24, 0, 0, 0)));
            Assert.IsTrue(period.IsActiveAt(new DateTime(2020, 7, 24, 0, 59, 59)));
            Assert.IsFalse(period.IsActiveAt(new DateTime(2020, 7, 24, 1, 0, 0)));
        }

        [TestMethod]
        public void CanCorrectlyDeterminePeriodIsActiveDuringTheSameDay()
        {
            var period = new NoComputerPeriod
            {
                StartTime = new TimeSpan(11, 0, 0),
                EndTime = new TimeSpan(13, 0, 0),
                DaysActive = DaySelection.Daily
            };

            Assert.IsFalse(period.IsActiveAt(new DateTime(2020, 7, 23, 10, 59, 59)));
            Assert.IsTrue(period.IsActiveAt(new DateTime(2020, 7, 23, 11, 0, 0)));
            Assert.IsTrue(period.IsActiveAt(new DateTime(2020, 7, 24, 12, 0, 0)));
            Assert.IsTrue(period.IsActiveAt(new DateTime(2020, 7, 24, 12, 59, 59)));
            Assert.IsFalse(period.IsActiveAt(new DateTime(2020, 7, 24, 13, 0, 0)));
        }

        [CsvTestMethod(typeof(DaySelectionConverterTestRow), @"Data\Models\Serialisation\DaySelectionConverterTestData.csv")]
        [DeploymentItem(@"Data\Models\Serialisation\DaySelectionConverterTestData.csv")]
        public void CanCorrectlyDeterminePeriodIsActiveOnSelectedDay(DaySelectionConverterTestRow row)
        {
            var period = new NoComputerPeriod
            {
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                DaysActive = new DaySelection
                {
                    Monday = row.Monday,
                    Tuesday = row.Tuesday,
                    Wednesday = row.Wednesday,
                    Thursday = row.Thursday,
                    Friday = row.Friday,
                    Saturday = row.Saturday,
                    Sunday = row.Sunday
                }
            };

            var monday = new DateTime(2020, 6, 29, 10, 30, 0);
            var tuesday = monday.AddDays(1);
            var wednesday = tuesday.AddDays(1);
            var thursday = wednesday.AddDays(1);
            var friday = thursday.AddDays(1);
            var saturday = friday.AddDays(1);
            var sunday = saturday.AddDays(1);

            Assert.AreEqual(row.Monday, period.IsActiveAt(monday));
            Assert.AreEqual(row.Tuesday, period.IsActiveAt(tuesday));
            Assert.AreEqual(row.Wednesday, period.IsActiveAt(wednesday));
            Assert.AreEqual(row.Thursday, period.IsActiveAt(thursday));
            Assert.AreEqual(row.Friday, period.IsActiveAt(friday));
            Assert.AreEqual(row.Saturday, period.IsActiveAt(saturday));
            Assert.AreEqual(row.Sunday, period.IsActiveAt(sunday));
        }

        [TestMethod]
        public void CanCorrectlyDeterminePeriodIsActiveOnDayAfterSelectedDayIfPeriodGoesPastMidnight()
        {
            var period = new NoComputerPeriod
            {
                StartTime = new TimeSpan(23, 0, 0),
                EndTime = new TimeSpan(2, 0, 0),
                DaysActive = new DaySelection
                {
                    Monday = true
                }
            };

            var monday = new DateTime(2020, 6, 29, 0, 0, 0);
            var tuesday = monday.AddDays(1);

            Assert.IsFalse(period.IsActiveAt(monday.Add(new TimeSpan(22, 59, 59))));
            Assert.IsTrue(period.IsActiveAt(monday.Add(new TimeSpan(23, 0, 0))));
            Assert.IsTrue(period.IsActiveAt(tuesday));
            Assert.IsTrue(period.IsActiveAt(tuesday.Add(new TimeSpan(1, 0, 0))));
            Assert.IsTrue(period.IsActiveAt(tuesday.Add(new TimeSpan(1, 59, 59))));
            Assert.IsFalse(period.IsActiveAt(tuesday.Add(new TimeSpan(2, 0, 0))));
            Assert.IsFalse(period.IsActiveAt(tuesday.Add(new TimeSpan(22, 59, 59))));
            Assert.IsFalse(period.IsActiveAt(tuesday.Add(new TimeSpan(23, 0, 0))));
        }
    }
}
