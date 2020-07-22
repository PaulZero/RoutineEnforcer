using Microsoft.Extensions.DependencyInjection;
using PaulZero.RoutineEnforcer.Services.Routine.Interfaces;
using PaulZero.RoutineEnforcer.Views.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PaulZero.RoutineEnforcer.Views.Controls
{
    /// <summary>
    /// Interaction logic for TaskSummary.xaml
    /// </summary>
    public partial class TaskSummary : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ScheduledEventViewModel> ScheduledTasks { get; }
            = new ObservableCollection<ScheduledEventViewModel>();

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
