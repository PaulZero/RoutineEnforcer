using Microsoft.Extensions.Logging;
using PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces;
using PaulZero.WindowsRoutine.Wpf.Services.Routine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock
{
    public class ClockService : IClockService
    {
        private Task _timingTask;

        private readonly List<TimedCallback> _callbacks = new List<TimedCallback>();
        private readonly IClockServiceCancellationProvider _cancellationProvider;
        private readonly ILogger _logger;
        private readonly IClockServiceTimeProvider _timeProvider;
        private DateTime _lastExecutionTime = default;

        public ClockService(ILogger<IClockService> logger) : this(new ClockServiceCancellationProvider(), logger, new ClockServiceTimeProvider())
        {
        }

        public ClockService(IClockServiceCancellationProvider cancellationProvider, ILogger<IClockService> logger, IClockServiceTimeProvider timeProvider)
        {
            _cancellationProvider = cancellationProvider ?? throw new ArgumentNullException(nameof(cancellationProvider));
            _logger = logger;
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public void RegisterCallback(TimedCallback callback)
        {
            if (callback is ScheduledEventTimedCallback s)
            {
                _logger.LogDebug($"Scheduled event '{s?.ScheduledEvent?.Name}' received and registered with clock service using ID '{s.Id}'.");
            }

            _callbacks.Add(callback);
        }

        public bool RemoveCallback(string callbackId)
        {
            try
            {
                _logger.LogDebug($"Removing callback referenced by ID {callbackId}");

                var existingCallback = _callbacks.FirstOrDefault(c => c.Id == callbackId);

                if (existingCallback == null)
                {
                    _logger.LogError($"Unable to find callback referenced by ID '{callbackId}'");

                    return false;
                }

                _callbacks.Remove(existingCallback);

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unable to remove callback referenced by ID {callbackId}");

                return false;
            }
        }

        public void RemoveAllCallbacks()
        {
            _logger.LogDebug($"Removing all callbacks from the clock service.");

            _callbacks.Clear();
        }

        public void Start()
        {
            _logger.LogDebug("Starting clock service.");

            _cancellationProvider.Prepare();

            _timingTask = Task.Run(RunLoop);
        }

        public void Stop()
        {
            _logger.LogDebug("Stopping clock service");

            _cancellationProvider.Cancel();
        }

        private async Task RunLoop()
        {
            _logger.LogDebug("Starting clock service loop");

            var currentTime = _timeProvider.GetCurrentTime();

            if (_lastExecutionTime == default)
            {
                _lastExecutionTime = currentTime;

                _logger.LogDebug($"No last execution time set, initialising to {_lastExecutionTime}");
            }

            if (currentTime.Second > 1)
            {
                InvokeDueCallbacks(currentTime);

                await CatchUpToNextMinute(currentTime);
            }

            while (_cancellationProvider.IsExecuting)
            {
                currentTime = _timeProvider.GetCurrentTime();

                if (currentTime.Day != _lastExecutionTime.Day)
                {
                    ResetDailyExecutionState();
                }

                InvokeDueCallbacks(currentTime);

                await Task.Delay(_timeProvider.GetClockInterval(), _cancellationProvider.Token);

                _lastExecutionTime = currentTime;
            }
        }

        private async Task CatchUpToNextMinute(DateTime currentTime)
        {
            _logger.LogDebug("Fast forwarding to next minute.");

            var nextMinute = currentTime.AddMinutes(1);

            await Task.Run(async () =>
            {
                while (currentTime.Minute != nextMinute.Minute)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));

                    currentTime = _timeProvider.GetCurrentTime();
                }
            });
        }

        private void InvokeDueCallbacks(DateTime currentTime)
        {
            foreach (var callback in _callbacks)
            {
                if (callback.IsDue(currentTime))
                {
                    _logger.LogDebug($"Invoking callback identified by ID '{callback.Id}'.");

                    callback.Invoke(_logger);
                }
            }
        }

        private void ResetDailyExecutionState()
        {
            foreach (var callback in _callbacks)
            {
                callback.ResetDailyExecutionState(_logger);
            }
        }

        public void Dispose()
        {
            _cancellationProvider?.Dispose();

            if (_timingTask != null)
            {
                if (_timingTask.IsCompleted)
                {
                    return;
                }

                _timingTask.Wait();
                _timingTask.Dispose();
            }
        }
    }
}
