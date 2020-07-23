using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using PaulZero.RoutineEnforcer.Services.ComputerControl.Interfaces;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using PaulZero.RoutineEnforcer.Services.Notifications.Interfaces;
using PaulZero.RoutineEnforcer.Services.Routine.Interfaces;
using PaulZero.RoutineEnforcer.Views.Models.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaulZero.RoutineEnforcer.Services.Routine
{
    internal class RoutineService : IRoutineService, IDisposable
    {
        private readonly IComputerControlService _actionService;
        private readonly IClockService _clockService;
        private readonly IConfigService _configService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;

        private readonly List<ScheduledEventTimedCallback> _timedScheduledEvents = new List<ScheduledEventTimedCallback>();
        private readonly List<NoComputerPeriodTimedCallback> _timedNoComputerPeriods = new List<NoComputerPeriodTimedCallback>();

        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public RoutineService(IComputerControlService actionService, IClockService clockService, IConfigService configService, INotificationService notificationService, ILogger<IRoutineService> logger)
        {
            _actionService = actionService;
            _clockService = clockService;
            _configService = configService;
            _logger = logger;
            _notificationService = notificationService;

            _configService.EventCreated += AddEventToClock;
            _configService.EventRemoved += RemoveEventFromClock;

            _configService.NoComputerPeriodCreated += AddNoComputerPeriodToClock;
            _configService.NoComputerPeriodRemoved += RemoveNoComputerPeriodFromClock;

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            _logger.LogDebug($"System has just resumed with mode {e.Mode}");

            if (e.Mode == PowerModes.Resume)
            {
                _logger.LogDebug("Restarting routine service...");

                Start();
            }
        }

        public void Dispose()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        }

        public TaskSummaryViewModel GetTaskOverview()
        {
            var scheduledEvents = _timedScheduledEvents.Select(t => t.ScheduledEvent);
            var noComputerPeriods = _timedNoComputerPeriods.Select(t => t.NoComputerPeriod);

            return new TaskSummaryViewModel(noComputerPeriods, scheduledEvents);
        }

        public void Start()
        {
            var config = _configService.GetAppConfiguration();

            _logger.LogDebug("Starting the routine service.");

            _clockService.RemoveAllCallbacks();

            foreach (var scheduledEvent in config.ScheduledEvents)
            {
                AddEventToClock(scheduledEvent);
            }

            foreach (var noComputerPeriod in config.NoComputerPeriods)
            {
                AddNoComputerPeriodToClock(noComputerPeriod);
            }

            _clockService.Start();
        }

        private void AddEventToClock(ScheduledEvent scheduledEvent)
        {
            try
            {
                _logger.LogDebug($"Adding scheduled event '{scheduledEvent?.Name}' to the clock service.");

                if (_timedScheduledEvents.Any(s => s.ScheduledEventId == scheduledEvent.Id))
                {
                    _logger.LogError($"The scheduled event '{scheduledEvent?.Name}' already exists in the clock service and won't be added.");

                    return;
                }

                var timedCallback = new ScheduledEventTimedCallback(scheduledEvent, ShowWarningNotification);

                _timedScheduledEvents.Add(timedCallback);

                _clockService.RegisterCallback(timedCallback);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to add scheduled event '{scheduledEvent?.Name}' to the clock service.");

                throw;
            }
        }

        private void AddNoComputerPeriodToClock(NoComputerPeriod noComputerPeriod)
        {
            try
            {
                _logger.LogDebug($"Adding no computer period '{noComputerPeriod?.Name}' to the clock service.");

                if (_timedNoComputerPeriods.Any(s => s.NoComputerPeriod.Id == noComputerPeriod.Id))
                {
                    _logger.LogError($"The no computer period '{noComputerPeriod?.Name}' already exists in the clock service and won't be added.");

                    return;
                }

                var timedCallback = new NoComputerPeriodTimedCallback(noComputerPeriod, ShowWarningNotification);

                _timedNoComputerPeriods.Add(timedCallback);

                _clockService.RegisterCallback(timedCallback);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to add no computer period '{noComputerPeriod?.Name}' to the clock service.");

                throw;
            }
        }

        private void RemoveEventFromClock(ScheduledEvent scheduledEvent)
        {
            var timedScheduledEvent = _timedScheduledEvents.FirstOrDefault(s => s.ScheduledEventId == scheduledEvent.Id);

            if (timedScheduledEvent != null)
            {
                _timedScheduledEvents.Remove(timedScheduledEvent);
                _clockService.RemoveCallback(scheduledEvent.Id);
            }
        }

        private void RemoveNoComputerPeriodFromClock(NoComputerPeriod noComputerPeriod)
        {
            var timedNoComputerPeriod = _timedNoComputerPeriods.FirstOrDefault(p => p.NoComputerPeriod.Id == noComputerPeriod.Id);

            if (timedNoComputerPeriod != null)
            {
                _timedNoComputerPeriods.Remove(timedNoComputerPeriod);
                _clockService.RemoveCallback(timedNoComputerPeriod.Id);
            }
        }

        private void ShowWarningNotification(ITimedCallback timedCallback)
        {
            if (timedCallback is ScheduledEventTimedCallback scheduledEventCallback)
            {
                ShowWarningNotification(scheduledEventCallback);
            }
            else if (timedCallback is NoComputerPeriodTimedCallback noComputerPeriodCallback)
            {
                ShowWarningNotification(noComputerPeriodCallback);
            }
        }

        private void ShowWarningNotification(ScheduledEventTimedCallback scheduledEventCallback)
        {
            _syncContext.Post(async s =>
            {
                await ShowWarningNotificationAsync(s as ScheduledEventTimedCallback);
            }, scheduledEventCallback);
        }

        private void ShowWarningNotification(NoComputerPeriodTimedCallback noComputerPeriodCallback)
        {
            _syncContext.Post(async s =>
            {
                await ShowWarningNotificationAsync(s as NoComputerPeriodTimedCallback);
            }, noComputerPeriodCallback);
        }

        private async Task ShowWarningNotificationAsync(NoComputerPeriodTimedCallback noComputerPeriodCallback)
        {
            var noComputerPeriod = noComputerPeriodCallback.NoComputerPeriod;

            var statusText = "Time until computer goes to sleep...";
            var title = "Sleep Pending";
            var skipButtonLabel = "Sleep Now";
            var message = $"{noComputerPeriod.Name}{Environment.NewLine}{Environment.NewLine}You can next use your computer at {noComputerPeriod.EndTime}";

            try
            {
                await _notificationService.ShowCountdownNotificationAsync(title, message, statusText, skipButtonLabel, noComputerPeriod.ActionDelay);
            }
            finally
            {
                noComputerPeriodCallback.MarkAsFinished();
            }

            _actionService.SleepComputer();
        }

        private async Task ShowWarningNotificationAsync(ScheduledEventTimedCallback scheduledEventCallback)
        {
            var scheduledEvent = scheduledEventCallback.ScheduledEvent;
            var isSleepAction = scheduledEvent.ActionType == EventActionType.SleepComputer;

            var statusText = isSleepAction ? "Time until computer goes to sleep..." : "Time until screen is locked...";
            var title = isSleepAction ? "Sleep Pending" : "Screen Lock Pending";
            var skipButtonLabel = isSleepAction ? "Sleep Now" : "Lock Screen Now";

            await _notificationService.ShowCountdownNotificationAsync(title, scheduledEvent.Name, statusText, skipButtonLabel, scheduledEvent.ActionDelay);

            if (scheduledEvent.ActionType == EventActionType.SleepComputer)
            {
                _actionService.SleepComputer();
            }
            else
            {
                _actionService.LockComputer();
            }
        }
    }
}
