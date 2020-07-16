using Microsoft.Extensions.DependencyInjection;
using PaulZero.WindowsRoutine.Wpf.Models.View;
using PaulZero.WindowsRoutine.Wpf.Services.Routine;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PaulZero.WindowsRoutine.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for TaskSummary.xaml
    /// </summary>
    public partial class TaskSummary : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ScheduledTaskViewModel> ScheduledTasks { get; }
            = new ObservableCollection<ScheduledTaskViewModel>();

        public bool HasScheduledTasks => ScheduledTasks.Any();

        public TaskSummary()
        {
            InitializeComponent();

            Loaded += TaskSummary_Loaded;
            ScheduledTasks.CollectionChanged += ScheduledTasks_CollectionChanged;
        }

        private void ScheduledTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasScheduledTasks)));
        }

        private void TaskSummary_Loaded(object sender, RoutedEventArgs e)
        {
            ScheduledTasks.Clear();

            foreach (var task in App.AppServices.GetService<IRoutineService>().GetTaskOverview())
            {
                ScheduledTasks.Add(task);
            }
        }
    }
}
