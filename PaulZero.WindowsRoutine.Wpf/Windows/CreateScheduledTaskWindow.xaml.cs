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
        public CreateScheduledTaskWindow()
        {
            InitializeComponent();
        }

        public ScheduledEvent CreateScheduledEvent()
        {
            var context = DataContext as CreateScheduledTaskViewModel;

            return new ScheduledEvent
            {
                Name = context.Name,
                Description = context.Description,
                WarningTime = context.SelectedTime,
                ActionDelay = TimeSpan.FromMinutes(context.MinutesDelay),
                ActionType = context.ActionType,
                DaysScheduled = context.DaysSelected
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
