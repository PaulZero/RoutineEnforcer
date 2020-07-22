using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Views.Models.Window;
using System;
using System.Windows;

namespace PaulZero.RoutineEnforcer.Views.Windows
{
    /// <summary>
    /// Interaction logic for CreateScheduledTaskWindow.xaml
    /// </summary>
    public partial class ScheduleEventWindow : Window
    {
        public ScheduleEventWindowViewModel ViewModel
        {
            get => DataContext as ScheduleEventWindowViewModel;
            set => DataContext = value;
        }

        public ScheduleEventWindow()
        {
            InitializeComponent();

            ViewModel.DialogResultSet += ViewModel_DialogResultSet;
        }

        private void ViewModel_DialogResultSet(bool dialogResult)
        {
            DialogResult = dialogResult;
        }

        public ScheduledEvent CreateScheduledEvent()
        {
            return new ScheduledEvent
            {
                Name = ViewModel.Name,
                WarningTime = ViewModel.SelectedTime,
                ActionDelay = TimeSpan.FromMinutes(ViewModel.MinutesDelay),
                ActionType = ViewModel.ActionType.ActionType,
                DaysScheduled = ViewModel.DaysSelected
            };
        }
    }
}
