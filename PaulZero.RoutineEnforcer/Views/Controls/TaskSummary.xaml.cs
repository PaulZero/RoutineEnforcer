using Microsoft.Extensions.DependencyInjection;
using PaulZero.RoutineEnforcer.Services.Routine.Interfaces;
using PaulZero.RoutineEnforcer.Views.Models.Controls;
using System.Windows;
using System.Windows.Controls;

namespace PaulZero.RoutineEnforcer.Views.Controls
{
    /// <summary>
    /// Interaction logic for TaskSummary.xaml
    /// </summary>
    public partial class TaskSummary : UserControl
    {
        public TaskSummaryViewModel ViewModel
        {
            get => DataContext as TaskSummaryViewModel;
            set => DataContext = value;
        }

        public TaskSummary()
        {
            InitializeComponent();

            ViewModel = new TaskSummaryViewModel();
        }

        private void TaskSummary_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = App.AppServices.GetService<IRoutineService>().GetTaskOverview();
        }
    }
}
