using PaulZero.WindowsRoutine.Wpf.Models.Commands;
using PaulZero.WindowsRoutine.Wpf.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PaulZero.WindowsRoutine.Wpf.Models.View
{
    public  class CreateScheduledTaskViewModel : AbstractViewModel
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
        public EventActionType ActionType
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

        public IEnumerable<EventActionType> AvailableActionTypes { get; } = new[]
        {
            EventActionType.LockScreen,
            EventActionType.SleepComputer
        };

        public ICommand CancelCommand => _cancelCommand;

        public ICommand ScheduleEventCommand => _scheduleEventCommand;

        private DaySelection _daysSelected;
        private string _description;
        private string _name;
        private TimeSpan _selectedTime;
        private int _minutesDelay = 15;
        private EventActionType _actionType = EventActionType.LockScreen;

        private readonly CallbackCommand _cancelCommand;
        private readonly CallbackCommand _scheduleEventCommand;

        public CreateScheduledTaskViewModel()
        {
            _cancelCommand = new CallbackCommand(Cancel);
            _scheduleEventCommand = new CallbackCommand(CanScheduleEvent, ScheduleEvent);

            SelectedTime = DateTime.Now.TimeOfDay;
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
