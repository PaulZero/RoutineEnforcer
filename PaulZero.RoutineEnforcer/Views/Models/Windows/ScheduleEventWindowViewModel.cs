using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Models.Validation;
using PaulZero.RoutineEnforcer.Views.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PaulZero.RoutineEnforcer.Views.Models.Windows
{
    public class ScheduleEventWindowViewModel : AbstractViewModel
    {
        public event Action<bool> DialogResultSet;

        public string Description
        {
            get => _description;
            set
            {
                _description = value;

                NotifyPropertyChanged();
            }
        }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;

                NotifyPropertyChanged();
            }
        }

        [Required]
        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;

                NotifyPropertyChanged();
            }
        }

        [Required]
        public int MinutesDelay
        {
            get => _minutesDelay;
            set
            {
                _minutesDelay = value;

                NotifyPropertyChanged();
            }
        }

        [Required]
        public EventActionTypeViewModel ActionType
        {
            get => _actionType;
            set
            {
                _actionType = value;

                NotifyPropertyChanged();
            }
        }

        [DaySelectionRequired]
        public DaySelection DaysSelected
        {
            get => _daysSelected;
            set
            {
                _daysSelected = value;

                NotifyPropertyChanged();
            }
        }

        public IEnumerable<int> AvailableMinutesDelay { get; } = Enumerable.Range(1, 60);

        public IEnumerable<EventActionTypeViewModel> AvailableActionTypes { get; } = new[]
        {
            new EventActionTypeViewModel(EventActionType.LockScreen),
            new EventActionTypeViewModel(EventActionType.SleepComputer)
        };

        public ICommand CancelCommand => _cancelCommand;

        public ICommand ScheduleEventCommand => _scheduleEventCommand;

        private readonly string _existingEventId;
        private DaySelection _daysSelected;
        private string _description;
        private string _name;
        private TimeSpan _selectedTime;
        private int _minutesDelay = 15;
        private EventActionTypeViewModel _actionType;

        private readonly CallbackCommand _cancelCommand;
        private readonly CallbackCommand _scheduleEventCommand;

        public ScheduleEventWindowViewModel()
        {
            _cancelCommand = new CallbackCommand(Cancel);
            _scheduleEventCommand = new CallbackCommand(CanScheduleEvent, ScheduleEvent);

            _selectedTime = DateTime.Now.TimeOfDay;
            _daysSelected = DaySelection.Daily;
            _actionType = AvailableActionTypes.First(t => t.ActionType == EventActionType.LockScreen);
        }

        public ScheduleEventWindowViewModel(ScheduledEventViewModel existingScheduledEvent)
        {
            _cancelCommand = new CallbackCommand(Cancel);
            _scheduleEventCommand = new CallbackCommand(CanScheduleEvent, ScheduleEvent);

            _existingEventId = existingScheduledEvent.Id;
            _name = existingScheduledEvent.Name;
            _selectedTime = existingScheduledEvent.WarningTime;

            var minutesDelay = (int)existingScheduledEvent.ActionDelay.TotalMinutes;

            if (minutesDelay < 0)
            {
                minutesDelay = 1;
            }
            else if (minutesDelay > 60)
            {
                minutesDelay = 60;
            }

            _minutesDelay = minutesDelay;
            _actionType = AvailableActionTypes.FirstOrDefault(a => a.ActionType == existingScheduledEvent.ActionType);
            _daysSelected = existingScheduledEvent.DaySelection;
        }

        public ScheduledEvent GetScheduledEvent()
        {
            var scheduledEvent = new ScheduledEvent
            {
                Name = Name,
                WarningTime = SelectedTime,
                ActionDelay = TimeSpan.FromMinutes(MinutesDelay),
                ActionType = ActionType.ActionType,
                DaysScheduled = DaysSelected
            };

            if (!string.IsNullOrWhiteSpace(_existingEventId))
            {
                scheduledEvent.Id = _existingEventId;
            }

            return scheduledEvent;
        }

        protected override void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);

            _scheduleEventCommand.Refresh();
        }

        private void Cancel(object parameter = default)
        {
            DialogResultSet.Invoke(false);
        }

        private bool CanScheduleEvent(object parameter = default)
        {
            return Error == string.Empty;
        }

        private void ScheduleEvent(object parameter = default)
        {
            DialogResultSet.Invoke(true);
        }
    }
}
