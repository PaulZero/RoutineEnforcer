using PaulZero.WindowsRoutine.Wpf.Models.View.Window;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace PaulZero.WindowsRoutine.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for FallbackNotificationWindow.xaml
    /// </summary>
    public partial class FallbackNotificationWindow : Window
    {
        public FallbackNotificationWindowViewModel ViewModel
        {
            get => DataContext as FallbackNotificationWindowViewModel;
            set => DataContext = value;
        }

        public FallbackNotificationWindow()
        {
            InitializeComponent();
        }

        public FallbackNotificationWindow(FallbackNotificationWindowViewModel viewModel) : this()
        {
            ViewModel = viewModel;
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
