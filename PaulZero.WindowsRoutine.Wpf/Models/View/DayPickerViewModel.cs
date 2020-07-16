namespace PaulZero.WindowsRoutine.Wpf.Models.View
{
    public class DayPickerViewModel : AbstractViewModel
    {
        public bool IsMondaySelected
        {
            get => _isMondaySelected; set
            {
                _isMondaySelected = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsTuesdaySelected
        {
            get => _isTuesdaySelected; set
            {
                _isTuesdaySelected = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsWednesdaySelected
        {
            get => _isWednesdaySelected; set
            {
                _isWednesdaySelected = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsThursdaySelected
        {
            get => _isThursdaySelected; set
            {
                _isThursdaySelected = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsFridaySelected
        {
            get => _isFridaySelected;
            set
            {
                _isFridaySelected = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsSaturdaySelected
        {
            get => _isSaturdaySelected; set
            {
                _isSaturdaySelected = value;

                NotifyPropertyChanged();
            }
        }

        public bool IsSundaySelected
        {
            get => _isSundaySelected; set
            {
                _isSundaySelected = value;

                NotifyPropertyChanged();
            }
        }

        public DaySelection CreateDaySelection()
        {
            return new DaySelection
            {
                Monday = IsMondaySelected,
                Tuesday = IsTuesdaySelected,
                Wednesday = IsWednesdaySelected,
                Thursday = IsThursdaySelected,
                Friday = IsFridaySelected,
                Saturday = IsSaturdaySelected,
                Sunday = IsSundaySelected
            };
        }

        private bool _isMondaySelected;
        private bool _isTuesdaySelected;
        private bool _isWednesdaySelected;
        private bool _isThursdaySelected;
        private bool _isFridaySelected;
        private bool _isSaturdaySelected;
        private bool _isSundaySelected;
    }
}
