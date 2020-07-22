using Microsoft.Extensions.Logging;
using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using PaulZero.RoutineEnforcer.Services.ComputerControl.Interfaces;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using PaulZero.RoutineEnforcer.Services.Notifications.Interfaces;
using PaulZero.RoutineEnforcer.Services.Routine.Interfaces;
using PaulZero.RoutineEnforcer.Views.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaulZero.RoutineEnforcer.Services.Routine
{
    internal class RoutineService : IRoutineService
    {
        private readonly IComputerControlService _actionService;
        private readonly IClockService _clockService;
        private readonly IConfigService _configService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly List<ScheduledEventTimedCallback> _timedScheduledEvents = new List<ScheduledEventTimedCallback>();
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public RoutineService(IComputerControlService actionService, IClockService clockService, IConfigService configService, INotificationService notificationService, ILogger<IRoutineService> logger)
        {
            _actionService = actionService;
            _clockService = clockService;
            _configService = configService;
            _logger = logger;
            _notificationService = notificationService;

            _configService.EventCreated += AddEventToTimingService;
            _configService.EventRemoved += RemoveEventFromTimingService;
        }

        public TimeSpan GetNextWarningCountdown()
        {
            return _configService.GetAppConfiguration().ScheduledEvents.Min(s => s.WarningDateTime) - DateTime.Now;
        }

        public ScheduledEventViewModel[] GetTaskOverview()
        {
            return _timedScheduledEvents
                .Select(s => new ScheduledEventViewModel(s.ScheduledEvent))
                .OrderBy(s => s.NextDueDate)
                .ToArray();
        }

        public void Start()
        {
            _logger.LogDebug("Starting the routine service.");

            _clockService.RemoveAllCallbacks();

            foreach (var scheduledEvent in _configService.GetAppConfiguration().ScheduledEvents)
            {
                AddEventToTimingService(scheduledEvent);
            }

            _clockService.Start();
        }

        private void AddEventToTimingService(ScheduledEvent scheduledEvent)
        {
            try
            {
                _logger.LogDebug($"Adding '{scheduledEvent?.Name}' to the clock service.");

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

        private void RemoveEventFromTimingService(ScheduledEvent scheduledEvent)
        {
            var timedScheduledEvent = _timedScheduledEvents.FirstOrDefault(s => s.ScheduledEventId == scheduledEvent.Id);

            if (timedScheduledEvent != null)
            {
                _timedScheduledEvents.Remove(timedScheduledEvent);
                _clockService.RemoveCallback(scheduledEvent.Id);
            }
        }

        private void ShowWarningNotification(ScheduledEvent scheduledEvent)
        {
            _syncContext.Post(async s =>
            {
                await ShowWarningNotificationAsync(s as ScheduledEvent);
            }, scheduledEvent);
        }

        private async Task ShowWarningNotificationAsync(ScheduledEvent scheduledEvent)
        {
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
