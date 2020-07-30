using Microsoft.Extensions.Logging;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using PaulZero.RoutineEnforcer.Services.Routine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PaulZero.RoutineEnforcer.Services.Clock
{
    public class ClockService : IClockService
    {
        private Task _timingTask;

        private readonly List<ITimedCallback> _callbacks = new List<ITimedCallback>();
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

        public void RegisterCallback(ITimedCallback callback)
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

        public void Restart()
        {
            _logger.LogDebug($"Restarting the clock service...");

            try
            {
                _cancellationProvider.Reset();

                _logger.LogDebug("Cancellation provider has been reset.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to reset cancellation provider: {exception.Message}");
            }

            var numCallbacks = _callbacks.Count;

            _callbacks.Clear();

            _logger.LogDebug($"Removed {numCallbacks} registered callback(s).");
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

                try
                {
                    await Task.Delay(_timeProvider.GetClockInterval(), _cancellationProvider.Token);
                }
                catch
                {
                    break;
                }

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
            var periodCallbacks = _callbacks.Where(c => c.IsPeriod);
            var eventCallbacks = _callbacks.Where(c => !c.IsPeriod);

            if (!CheckAndInvokeCallbacks(periodCallbacks, currentTime))
            {
                CheckAndInvokeCallbacks(eventCallbacks, currentTime);
            }
        }

        private bool CheckAndInvokeCallbacks(IEnumerable<ITimedCallback> eventCallbacks, DateTime currentTime)
        {
            foreach (var callback in eventCallbacks)
            {
                if (callback.IsDue(currentTime))
                {
                    _logger.LogDebug($"Invoking callback identified by ID '{callback.Id}'.");

                    if (callback.IsPeriod && callback.IsExecuting)
                    {
                        _logger.LogDebug("The callback is periodic and still flagged as executing, skipping all execution for now.");

                        return true;
                    }

                    callback.Invoke(_logger);

                    return true;
                }
            }

            return false;
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

            _timingTask?.Wait();
            _timingTask?.Dispose();
        }
    }
}
