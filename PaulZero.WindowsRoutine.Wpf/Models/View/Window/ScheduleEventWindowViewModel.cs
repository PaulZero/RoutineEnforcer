using PaulZero.WindowsRoutine.Wpf.Models.Commands;
using PaulZero.WindowsRoutine.Wpf.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PaulZero.WindowsRoutine.Wpf.Models.View.Window
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

            SelectedTime = DateTime.Now.TimeOfDay;

            _actionType = AvailableActionTypes.First(t => t.ActionType == EventActionType.LockScreen);
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
