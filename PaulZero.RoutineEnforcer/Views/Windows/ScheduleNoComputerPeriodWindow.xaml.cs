using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Views.Models;
using PaulZero.RoutineEnforcer.Views.Models.Windows;
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
            : this(new ScheduleNoComputerPeriodWindowViewModel())
        {
        }

        public ScheduleNoComputerPeriodWindow(NoComputerPeriodViewModel existingNoComputerPeriod)
            : this(new ScheduleNoComputerPeriodWindowViewModel(existingNoComputerPeriod))
        {
        }

        public ScheduleNoComputerPeriodWindow(ScheduleNoComputerPeriodWindowViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            ViewModel.DialogResultSet += ViewModel_DialogResultSet;
        }

        private void ViewModel_DialogResultSet(bool dialogResult)
        {
            DialogResult = dialogResult;
        }

        public NoComputerPeriod CreateNoComputerPeriod()
            => ViewModel.GetNoComputerPeriod();
    }
}
