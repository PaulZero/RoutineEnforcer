using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaulZero.RoutineEnforcer.Wpf.Services.Clock;
using System;

namespace PaulZero.RoutineEnforcer.Tests.Services.Clock
{
    [TestClass]
    public class TimedCallbackTest
    {
        [DataTestMethod]
        [DataRow(-1, true)]
        [DataRow(0, false)]
        [DataRow(23, false)]
        [DataRow(24, true)]
        public void TestThatHoursValidateCorrectly(int hour, bool expectsException)
        {
            try
            {
                new TimedCallback(() => { }, hour, 25);

                if (expectsException)
                {
                    Assert.Fail($"An hour value of {hour} should have thrown an {nameof(ArgumentOutOfRangeException)}!");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                if (!expectsException)
                {
                    Assert.Fail($"An hour value of {hour} should NOT have thrown an {nameof(ArgumentOutOfRangeException)}!");
                }
            }
            catch (Exception exception)
            {
                Assert.Fail($"An hour value of {hour} should not have thrown an instance of {exception.GetType().Name}!");
            }
        }

        [DataTestMethod]
        [DataRow(-1, true)]
        [DataRow(0, false)]
        [DataRow(59, false)]
        [DataRow(60, true)]
        public void TestThatMinutesValidateCorrectly(int minute, bool expectsException)
        {
            try
            {
                new TimedCallback(() => { }, 12, minute);

                if (expectsException)
                {
                    Assert.Fail($"A minute value of {minute} should have thrown an {nameof(ArgumentOutOfRangeException)}!");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                if (!expectsException)
                {
                    Assert.Fail($"A minute value of {minute} should NOT have thrown an {nameof(ArgumentOutOfRangeException)}!");
                }
            }
            catch (Exception exception)
            {
                Assert.Fail($"A minute value of {minute} should not have thrown an instance of {exception.GetType().Name}!");
            }
        }

        [TestMethod]
        public void TestThatIsDueOnlyReturnsTrueWhenCallbackIsActuallyDue()
        {
            var actualHour = 12;
            var actualMinute = 37;

            var betterCallback = new TimedCallback(() => { }, actualHour, actualMinute);

            var currentDateTime = DateTime.Today;
            var endDateTime = currentDateTime.AddDays(1);

            while (currentDateTime < endDateTime)
            {
                currentDateTime = currentDateTime.AddMinutes(1);

                var shouldBeDue = currentDateTime.Hour == actualHour && currentDateTime.Minute == actualMinute;

                Assert.AreEqual(shouldBeDue, betterCallback.IsDue(currentDateTime));
            }
        }



        [TestMethod]
        public void TestThatCallbackIsNotAllowedToBeNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TimedCallback(null, 12, 12));
        }

        [TestMethod]
        public void TestThatCallbackIsExecutedWhenInvokeIsCalled()
        {
            var hasCallbackBeenExecuted = false;

            var callback = new TimedCallback(() => hasCallbackBeenExecuted = true, 12, 12);

            callback.Invoke(new NullLogger<TimedCallback>());

            Assert.IsTrue(hasCallbackBeenExecuted);
        }
    }
}
