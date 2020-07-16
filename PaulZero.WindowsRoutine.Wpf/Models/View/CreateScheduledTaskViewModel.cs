using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PaulZero.WindowsRoutine.Wpf.Models.View
{
    internal class CreateScheduledTaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Description
        {
            get => _description;
            set
            {
                _description = value;

                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;

                NotifyPropertyChanged();
            }
        }

        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;

                NotifyPropertyChanged();
            }
        }

        public int MinutesDelay
        {
            get => _minutesDelay;
            set
            {
                _minutesDelay = value;

                NotifyPropertyChanged();
            }
        }

        public EventActionType ActionType
        {
            get => _actionType;
            set
            {
                _actionType = value;

                NotifyPropertyChanged();
            }
        }

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

        private DaySelection _daysSelected;
        private string _description;
        private string _name;
        private TimeSpan _selectedTime;
        private int _minutesDelay = 15;
        private EventActionType _actionType = EventActionType.LockScreen;

        public CreateScheduledTaskViewModel()
        {
            DaysSelected = new DaySelection();
            SelectedTime = DateTime.Now.TimeOfDay;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
