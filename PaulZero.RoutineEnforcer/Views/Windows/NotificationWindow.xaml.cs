using PaulZero.RoutineEnforcer.Views.Models.Windows;
using System.Windows;

namespace PaulZero.RoutineEnforcer.Views.Windows
{
    /// <summary>
    /// Interaction logic for FallbackNotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public NotificationWindowViewModel ViewModel
        {
            get => DataContext as NotificationWindowViewModel;
            set => DataContext = value;
        }

        public NotificationWindow()
        {
            InitializeComponent();
        }

        public NotificationWindow(NotificationWindowViewModel viewModel) : this()
        {
            ViewModel = viewModel;
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
