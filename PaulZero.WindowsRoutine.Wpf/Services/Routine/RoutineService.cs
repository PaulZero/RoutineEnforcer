using PaulZero.WindowsRoutine.Wpf.Models;
using PaulZero.WindowsRoutine.Wpf.Models.View;
using PaulZero.WindowsRoutine.Wpf.Services.Actions;
using PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces;
using PaulZero.WindowsRoutine.Wpf.Services.Config;
using PaulZero.WindowsRoutine.Wpf.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaulZero.WindowsRoutine.Wpf.Services.Routine
{
    internal class RoutineService : IRoutineService
    {
        private readonly IActionService _actionService;
        private readonly IClockService _clockService;
        private readonly IConfigService _configService;
        private readonly INotificationService _notificationService;
        private readonly List<ScheduledEventTimedCallback> _timedScheduledEvents = new List<ScheduledEventTimedCallback>();
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public RoutineService(IActionService actionService, IClockService clockService, IConfigService configService, INotificationService notificationService)
        {
            _actionService = actionService;
            _clockService = clockService;
            _configService = configService;
            _notificationService = notificationService;

            _configService.EventCreated += AddEventToTimingService;
            _configService.EventRemoved += RemoveEventFromTimingService;
        }

        public TimeSpan GetNextWarningCountdown()
        {
            return _configService.GetAppConfiguration().ScheduledEvents.Min(s => s.WarningDateTime) - DateTime.Now;
        }

        public ScheduledTaskViewModel[] GetTaskOverview()
            => _timedScheduledEvents
                .Select(s => new ScheduledTaskViewModel(s.ScheduledEvent))
                .OrderBy(s => s.NextDueDate)
                .ToArray();

        public void Start()
        {
            _clockService.RemoveAllCallbacks();

            var first = true;

            foreach (var scheduledEvent in _configService.GetAppConfiguration().ScheduledEvents)
            {
                AddEventToTimingService(scheduledEvent);
            }

            _clockService.Start();
        }

        private void AddEventToTimingService(ScheduledEvent scheduledEvent)
        {
            if (_timedScheduledEvents.Any(s => s.ScheduledEventId == scheduledEvent.Id))
            {
                throw new Exception($"A scheduled event already exists in the timing service with that ID!");
            }

            var timedCallback = new ScheduledEventTimedCallback(scheduledEvent, ShowWarningNotification);

            _timedScheduledEvents.Add(timedCallback);

            _clockService.RegisterCallback(timedCallback);
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

            await _notificationService.ShowCountdownNotificationAsync(Guid.NewGuid(), title, scheduledEvent.Name, statusText, skipButtonLabel, scheduledEvent.ActionDelay);

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
