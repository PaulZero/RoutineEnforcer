using Hardcodet.Wpf.TaskbarNotification;
using PaulZero.RoutineEnforcer.Views.Models.Windows;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;

namespace PaulZero.RoutineEnforcer.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindowViewModel ViewModel
        {
            get => DataContext as MainWindowViewModel;
            set => DataContext = value;
        }

        public bool HasLoadingError
        {
            get => _hasLoadingError;
            set
            {
                _hasLoadingError = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasLoadingError)));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }

        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _hasLoadingError;
        private string _statusMessage = "Loading...";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Load();

            using var imageStream = new MemoryStream(Resource.clock_icon);

            TaskbarIcon.Icon = new Icon(imageStream);
        }
    }
}
