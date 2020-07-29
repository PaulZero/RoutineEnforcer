using PaulZero.RoutineEnforcer.Views.Models.Windows;
using System.Windows;

namespace PaulZero.RoutineEnforcer.Views.Windows
{
    /// <summary>
    /// Interaction logic for ManageScheduleWindow.xaml
    /// </summary>
    public partial class ManageScheduleWindow : Window
    {
        public ManageScheduleWindowViewModel ViewModel
        {
            get => DataContext as ManageScheduleWindowViewModel;
            set => DataContext = value;
        }

        public ManageScheduleWindow() : this(new ManageScheduleWindowViewModel())
        {
        }

        public ManageScheduleWindow(ManageScheduleWindowViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }
    }
}
