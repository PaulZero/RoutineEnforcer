using PaulZero.WindowsRoutine.Wpf.Models;
using PaulZero.WindowsRoutine.Wpf.Models.View;
using System;
using System.Windows;

namespace PaulZero.WindowsRoutine.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for CreateScheduledTaskWindow.xaml
    /// </summary>
    public partial class CreateScheduledTaskWindow : Window
    {
        public CreateScheduledTaskViewModel ViewModel
        {
            get => DataContext as CreateScheduledTaskViewModel;
            set => DataContext = value;
        }

        public CreateScheduledTaskWindow()
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
                Description = ViewModel.Description,
                WarningTime = ViewModel.SelectedTime,
                ActionDelay = TimeSpan.FromMinutes(ViewModel.MinutesDelay),
                ActionType = ViewModel.ActionType,
                DaysScheduled = ViewModel.DaysSelected
            };
        }
    }
}
