﻿using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaulZero.RoutineEnforcer.Services.Clock;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaulZero.RoutineEnforcer.Tests.Services.Clock
{
    [TestClass]
    public class ClockServiceTest
    {
        [TestMethod]
        public async Task TestThatCorrectlyGeneratesTicksAtExpectedIntervals()
        {
            // TODO: Address occasional failures related to cancellation being requested, suspect I'm not trying to catch a task being aborted.

            var startTime = DateTime.Today;
            var cancellationProvider = new TestClockServiceCancellationProvider();
            var timeProvider = new TestClockServiceTimeProvider(TimeSpan.FromTicks(100), startTime, cancellationProvider, startTime.AddDays(1));
            using var clockService = new ClockService(cancellationProvider, new NullLogger<IClockService>(), timeProvider);

            var currentTime = startTime;
            var endTime = currentTime.AddDays(1);

            var timedCallbackExecutionState = new Dictionary<int, bool>();
            var timedCallbacks = new Dictionary<int, AbstractTimedCallback>();

            for (var taskIndex = 0; taskIndex < 1440; taskIndex++)
            {
                var actualIndex = taskIndex;

                var callback = new Action<ITimedCallback>(c =>
                {
                    if (timedCallbackExecutionState[actualIndex])
                    {
                        return;
                    }

                    timedCallbackExecutionState[actualIndex] = true;
                });

                var timedCallback = new TestEventCallback(currentTime.Hour, currentTime.Minute, callback);

                timedCallbacks.Add(actualIndex, timedCallback);
                timedCallbackExecutionState.Add(actualIndex, false);

                clockService.RegisterCallback(timedCallback);

                currentTime = currentTime.AddMinutes(1);
            }

            clockService.Start();

            await cancellationProvider.WaitForCancellationAsync();

            Assert.IsTrue(timedCallbackExecutionState.All(kvp => kvp.Value));
            Assert.IsTrue(timedCallbacks.All(kvp => kvp.Value.DailyExecutionState == TimedCallbackExecutionState.RanSuccessfully));
            Assert.IsTrue(timedCallbacks.All(kvp => kvp.Value.LastExecutionState == TimedCallbackExecutionState.RanSuccessfully));
        }

        [TestMethod]
        public async Task TestThatPeriodCallbacksWillNotRunConcurrently()
        {
            var startTime = DateTime.Today;
            var cancellationProvider = new TestClockServiceCancellationProvider();
            var timeProvider = new TestClockServiceTimeProvider(TimeSpan.FromTicks(100), startTime, cancellationProvider, startTime.AddMinutes(5));
            using var clockService = new ClockService(cancellationProvider, new NullLogger<IClockService>(), timeProvider);

            var timesCalled = 0;
            var callback = new Action<ITimedCallback>(c => { timesCalled++; });

            clockService.RegisterCallback(new TestPeriodCallback(callback));

            clockService.Start();

            await cancellationProvider.WaitForCancellationAsync();

            Assert.AreEqual(1, timesCalled);
        }


        [TestMethod]
        public async Task TestThatClockServiceCorrectlyStopsAndRestarts()
        {
            // TODO: Figure out why this intermittently fails, stupid time...

            var startTime = DateTime.Today;

            var cancellationProvider = new TestClockServiceCancellationProvider();
            var timeProvider = new TestClockServiceTimeProvider(TimeSpan.FromTicks(100), startTime, cancellationProvider, startTime.AddDays(30));
            using var clockService = new ClockService(cancellationProvider, new NullLogger<IClockService>(), timeProvider);

            clockService.Start();

            await timeProvider.WaitUntilDateTimeAsync(startTime.AddDays(1));

            clockService.Stop();

            var firstStopIterations = timeProvider.GetCurrentTimeCallCount;

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(firstStopIterations, timeProvider.GetCurrentTimeCallCount);

            clockService.Start();

            await timeProvider.WaitUntilDateTimeAsync(timeProvider.CurrentTime.AddDays(1));

            clockService.Stop();

            var secondStopIterations = timeProvider.GetCurrentTimeCallCount;

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(secondStopIterations, timeProvider.GetCurrentTimeCallCount);

            Assert.IsTrue(secondStopIterations > firstStopIterations);
        }

        [TestMethod]
        public async Task TestThatDailyExecutionStateResetCorrectly()
        {
            var startTime = DateTime.Today;

            var cancellationProvider = new TestClockServiceCancellationProvider();
            var timeProvider = new TestClockServiceTimeProvider(TimeSpan.FromTicks(100), startTime, cancellationProvider, startTime.AddDays(1));
            using var clockService = new ClockService(cancellationProvider, new NullLogger<IClockService>(), timeProvider);

            var currentTime = startTime.AddHours(1); // Start an hour in so there's no awful race condition for cancellation
            var endTime = startTime.AddDays(1);
            var timedCallbackCount = (endTime - currentTime).TotalMinutes;
            var timedCallbacks = new List<AbstractTimedCallback>();

            for (var taskIndex = 0; taskIndex < timedCallbackCount; taskIndex++)
            {
                var actualIndex = taskIndex;

                var timedCallback = new TestEventCallback(currentTime.Hour, currentTime.Minute);

                timedCallbacks.Add(timedCallback);

                clockService.RegisterCallback(timedCallback);

                currentTime = currentTime.AddMinutes(1);
            }

            clockService.Start();

            await cancellationProvider.WaitForCancellationAsync();

            Assert.IsTrue(timedCallbacks.All(c => c.DailyExecutionState == TimedCallbackExecutionState.RanSuccessfully));
            Assert.IsTrue(timedCallbacks.All(c => c.LastExecutionState == TimedCallbackExecutionState.RanSuccessfully));

            timeProvider.AddToCancellationDateTime(TimeSpan.FromMinutes(5));

            clockService.Start();

            await cancellationProvider.WaitForCancellationAsync();

            Assert.IsTrue(timedCallbacks.All(c => c.DailyExecutionState == TimedCallbackExecutionState.HasNotRun));
            Assert.IsTrue(timedCallbacks.All(c => c.LastExecutionState == TimedCallbackExecutionState.RanSuccessfully));
        }

        public class TestEventCallback : AbstractTimedCallback
        {
            public override bool IsPeriod => false;

            public int Hour { get; set; }

            public int Minute { get; set; }

            public TestEventCallback(int hour, int minute)
                : this (hour, minute, c => { })
            {
            }

            public TestEventCallback(int hour, int minute, Action<ITimedCallback> callback)
                : base(callback)
            {
                Hour = hour;
                Minute = minute;
            }

            public override bool IsDue(DateTime currentDateTime)
            {
                return currentDateTime.Hour == Hour && currentDateTime.Minute == Minute;
            }
        }

        public class TestPeriodCallback : AbstractTimedCallback
        {
            public TestPeriodCallback(Action<ITimedCallback> callback)
                : base(callback)
            {
            }

            public override bool IsPeriod => true;

            public override bool IsDue(DateTime currentDateTime)
            {
                return true;
            }
        }

        public class TestClockServiceTimeProvider : IClockServiceTimeProvider
        {
            private readonly TimeSpan _clockInterval;
            private DateTime _currentTime;
            private readonly DateTime _startTime;

            public DateTime CurrentTime => _currentTime;

            public int GetCurrentTimeCallCount { get; private set; }

            private readonly IClockServiceCancellationProvider _cancellationProvider;
            private DateTime _cancellationDateTime;

            public TestClockServiceTimeProvider(TimeSpan clockInterval, DateTime currentTime, IClockServiceCancellationProvider cancellationProvider, DateTime cancellationDateTime)
            {
                _clockInterval = clockInterval;
                _currentTime = currentTime;
                _startTime = currentTime;

                _cancellationProvider = cancellationProvider;
                _cancellationDateTime = cancellationDateTime;
            }

            public TimeSpan GetClockInterval()
            {
                _currentTime = _currentTime.AddMinutes(1);

                if (_currentTime >= _cancellationDateTime)
                {
                    _cancellationProvider.Cancel();
                }

                GetCurrentTimeCallCount++;

                return _clockInterval;
            }

            public DateTime GetCurrentTime() => _currentTime;

            public async Task WaitUntilDateTimeAsync(DateTime dateTime)
            {
                await Task.Run(async () =>
                {
                    while (_currentTime < dateTime)
                    {
                        await Task.Delay(TimeSpan.FromTicks(10));
                    }
                });
            }

            public void AddToCancellationDateTime(TimeSpan addition)
            {
                _cancellationDateTime = _cancellationDateTime.Add(addition);
            }

            public async Task WaitForDayToEndAsync()
            {
                await WaitUntilDateTimeAsync(_startTime.AddDays(1));
            }
        }

        public class TestClockServiceCancellationProvider : IClockServiceCancellationProvider
        {
            public bool IsExecuting
            {
                get
                {
                    if (_cancellationTokenSource == null)
                    {
                        return false;
                    }

                    return !_cancellationTokenSource.IsCancellationRequested;
                }
            }

            public CancellationToken Token => _cancellationTokenSource?.Token ?? CancellationToken.None;

            private CancellationTokenSource _cancellationTokenSource;

            public void Cancel()
            {
                if (_cancellationTokenSource != null)
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Cancel();
                    }

                    _cancellationTokenSource?.Dispose();
                }
            }

            public void Prepare()
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            public async Task WaitForCancellationAsync()
            {
                await Task.Run(async () =>
                {
                    while (IsExecuting)
                    {
                        await Task.Delay(TimeSpan.FromTicks(100));
                    }
                });
            }

            public void Dispose()
            {
                _cancellationTokenSource?.Dispose();
            }
        }
    }
}
