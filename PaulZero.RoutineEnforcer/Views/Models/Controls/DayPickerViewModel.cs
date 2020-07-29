using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Views.Commands;
using System.Windows.Input;

namespace PaulZero.RoutineEnforcer.Views.Models.Controls
{
    public class DayPickerViewModel : AbstractViewModel
    {
        public DaySelection Selection
        {
            get => _daySelection;
            set
            {
                _daySelection = value;

                IsAllDaysChecked = value.Equals(DaySelection.Daily);

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsMondaySelected));
                NotifyPropertyChanged(nameof(IsTuesdaySelected));
                NotifyPropertyChanged(nameof(IsWednesdaySelected));
                NotifyPropertyChanged(nameof(IsThursdaySelected));
                NotifyPropertyChanged(nameof(IsFridaySelected));
                NotifyPropertyChanged(nameof(IsSaturdaySelected));
                NotifyPropertyChanged(nameof(IsSundaySelected));
            }
        }

        public bool IsMondaySelected
        {
            get => Selection.Monday;
            set
            {
                Selection = Selection.WithMonday(value);
            }
        }

        public bool IsTuesdaySelected
        {
            get => Selection.Tuesday;
            set
            {
                Selection = Selection.WithTuesday(value);
            }
        }

        public bool IsWednesdaySelected
        {
            get => Selection.Wednesday;
            set
            {
                Selection = Selection.WithWednesday(value);
            }
        }

        public bool IsThursdaySelected
        {
            get => Selection.Thursday;
            set
            {
                Selection = Selection.WithThursday(value);
            }
        }

        public bool IsFridaySelected
        {
            get => Selection.Friday;
            set
            {
                Selection = Selection.WithFriday(value);
            }
        }

        public bool IsSaturdaySelected
        {
            get => Selection.Saturday;
            set
            {
                Selection = Selection.WithSaturday(value);
            }
        }

        public bool IsSundaySelected
        {
            get => Selection.Sunday;
            set
            {
                Selection = Selection.WithSunday(value);
            }
        }

        public bool IsAllDaysChecked
        {
            get => _isAllDaysChecked;
            set
            {
                if (_isAllDaysChecked == value)
                {
                    return;
                }

                _isAllDaysChecked = value;

                _selectAllDaysCommand.Refresh();
                _selectSpecificDaysCommand.Refresh();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsSpecificDaysChecked));
            }
        }

        public bool IsSpecificDaysChecked
        {
            get => !IsAllDaysChecked;
            set
            {
                IsAllDaysChecked = !value;
            }
        }

        public ICommand SelectAllDaysCommand => _selectAllDaysCommand;

        public ICommand SelectSpecificDaysCommand => _selectSpecificDaysCommand;

        private bool _isAllDaysChecked;
        private readonly CallbackCommand _selectAllDaysCommand;
        private readonly CallbackCommand _selectSpecificDaysCommand;
        private DaySelection _daySelection;

        public DayPickerViewModel()
        {
            _selectAllDaysCommand = new CallbackCommand(CanSelectAllDays, DoSelectAllDays);
            _selectSpecificDaysCommand = new CallbackCommand(CanSelectSpecificDays, DoSelectSpecificDays);

            //DoSelectAllDays();
        }

        private bool CanSelectAllDays(object parameter = default)
        {
            return IsSpecificDaysChecked;
        }

        private void DoSelectAllDays(object parameter = default)
        {
            IsAllDaysChecked = true;

            Selection = DaySelection.Daily;
        }

        private bool CanSelectSpecificDays(object parameter = default)
        {
            return IsAllDaysChecked;
        }

        private void DoSelectSpecificDays(object parameter = default)
        {
            IsSpecificDaysChecked = true;

            Selection = DaySelection.Empty;
        }
    }
}
