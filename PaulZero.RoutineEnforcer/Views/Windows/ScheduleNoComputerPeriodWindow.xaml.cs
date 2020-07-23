using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Views.Models.Windows;
using System;
using System.Windows;

namespace PaulZero.RoutineEnforcer.Views.Windows
{
    /// <summary>
    /// Interaction logic for ScheduleNoComputerPeriodWindow.xaml
    /// </summary>
    public partial class ScheduleNoComputerPeriodWindow : Window
    {
        public ScheduleNoComputerPeriodWindowViewModel ViewModel
        {
            get => DataContext as ScheduleNoComputerPeriodWindowViewModel;
            set => DataContext = value;
        }

        public ScheduleNoComputerPeriodWindow()
        {
            InitializeComponent();

            ViewModel.DialogResultSet += ViewModel_DialogResultSet;
        }

        private void ViewModel_DialogResultSet(bool dialogResult)
        {
            DialogResult = dialogResult;
        }

        public NoComputerPeriod CreatedNoComputerPeriod()
        {
            return new NoComputerPeriod
            {
                Name = ViewModel.Name,
                StartTime = ViewModel.StartTime,
                EndTime = ViewModel.EndTime,
                DaysActive = ViewModel.DaysSelected,
                ActionDelay = TimeSpan.FromMinutes(ViewModel.MinutesDelay)
            };
        }
    }
}
