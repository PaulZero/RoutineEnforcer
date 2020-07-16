using PaulZero.WindowsRoutine.Wpf.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PaulZero.WindowsRoutine.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for DayPicker.xaml
    /// </summary>
    public partial class DayPicker : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty DaysSelectedProperty =
            DependencyProperty.Register(nameof(DaysSelected), typeof(DaySelection), typeof(DayPicker), new PropertyMetadata(new DaySelection()));

        public DaySelection DaysSelected
        {
            get { return GetValue(DaysSelectedProperty) as DaySelection; }
            set
            {
                SetValue(DaysSelectedProperty, value);

                NotifyPropertyChanged();
            }
        }

        public bool IsEveryDay
        {
            get => _isEveryDay;
            set
            {
                _isEveryDay = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsSelectedDays));

                IsMondaySelected = value;
                IsTuesdaySelected = value;
                IsWednesdaySelected = value;
                IsThursdaySelected = value;
                IsFridaySelected = value;
                IsSaturdaySelected = value;
                IsSundaySelected = value;
            }
        }

        public bool IsSelectedDays
        {
            get => !IsEveryDay;
            set
            {
                IsEveryDay = !value;
            }
        }

        public bool IsMondaySelected
        {
            get => DaysSelected?.Monday ?? false;
            set
            {
                var daysSelected = DaysSelected ?? new DaySelection();

                daysSelected.Monday = value;

                DaysSelected = daysSelected;

                NotifyPropertyChanged();
            }
        }

        public bool IsTuesdaySelected
        {
            get => DaysSelected?.Tuesday ?? false;
            set
            {
                var daysSelected = DaysSelected ?? new DaySelection();

                daysSelected.Tuesday = value;

                DaysSelected = daysSelected;

                NotifyPropertyChanged();
            }
        }

        public bool IsWednesdaySelected
        {
            get => DaysSelected?.Wednesday ?? false;
            set
            {
                var daysSelected = DaysSelected ?? new DaySelection();

                daysSelected.Wednesday = value;

                DaysSelected = daysSelected;

                NotifyPropertyChanged();
            }
        }

        public bool IsThursdaySelected
        {
            get => DaysSelected?.Thursday ?? false;
            set
            {
                var daysSelected = DaysSelected ?? new DaySelection();

                daysSelected.Thursday = value;

                DaysSelected = daysSelected;

                NotifyPropertyChanged();
            }
        }

        public bool IsFridaySelected
        {
            get => DaysSelected?.Friday ?? false;
            set
            {
                var daysSelected = DaysSelected ?? new DaySelection();

                daysSelected.Friday = value;

                DaysSelected = daysSelected;

                NotifyPropertyChanged();
            }
        }

        public bool IsSaturdaySelected
        {
            get => DaysSelected?.Saturday ?? false;
            set
            {
                var daysSelected = DaysSelected ?? new DaySelection();

                daysSelected.Saturday = value;

                DaysSelected = daysSelected;

                NotifyPropertyChanged();
            }
        }

        public bool IsSundaySelected
        {
            get => DaysSelected?.Sunday ?? false;
            set
            {
                var daysSelected = DaysSelected ?? new DaySelection();

                daysSelected.Sunday = value;

                DaysSelected = daysSelected;

                NotifyPropertyChanged();
            }
        }

        private bool _isEveryDay = true;

        public DayPicker()
        {
            InitializeComponent();

            IsEveryDay = true;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
