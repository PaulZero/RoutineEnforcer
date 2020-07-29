using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Views.Models;
using PaulZero.RoutineEnforcer.Views.Models.Windows;
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
            : this(new ScheduleEventWindowViewModel())
        {
        }

        public ScheduleEventWindow(ScheduledEventViewModel existingScheduledEvent)
            : this(new ScheduleEventWindowViewModel(existingScheduledEvent))
        {
        }

        public ScheduleEventWindow(ScheduleEventWindowViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            ViewModel.DialogResultSet += ViewModel_DialogResultSet;
        }

        private void ViewModel_DialogResultSet(bool dialogResult)
        {
            DialogResult = dialogResult;
        }

        public ScheduledEvent CreateScheduledEvent()
            => ViewModel.GetScheduledEvent();
    }
}
