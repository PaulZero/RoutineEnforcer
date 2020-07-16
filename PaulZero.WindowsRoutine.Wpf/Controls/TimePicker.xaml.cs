using PaulZero.WindowsRoutine.Wpf.Models.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PaulZero.WindowsRoutine.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty SelectedTimeProperty =
               DependencyProperty.Register("SelectedTime", typeof(TimeSpan), typeof(TimePicker), new PropertyMetadata(TimeSpan.Zero));

        public TimeSpan SelectedTime
        {
            get { return (TimeSpan)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        public int Hours
        {
            get => SelectedTime.Hours;
            set
            {
                var currentTime = SelectedTime;

                SelectedTime = new TimeSpan(value, currentTime.Minutes, 0);

                NotifyPropertyChanged();
            }
        }

        public int Minutes
        {
            get => SelectedTime.Minutes;
            set
            {
                var currentTime = SelectedTime;

                SelectedTime = new TimeSpan(currentTime.Hours, value, 0);

                NotifyPropertyChanged();
            }
        }

        public IEnumerable<int> AvailableHours { get; } = Enumerable.Range(0, 24);

        public IEnumerable<int> AvailableMinutes { get; } = Enumerable.Range(0, 60);

        public TimePicker()
        {
            InitializeComponent();

            Hours = DateTime.Now.Hour;
            Minutes = DateTime.Now.Minute;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
