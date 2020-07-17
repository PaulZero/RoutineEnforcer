using PaulZero.WindowsRoutine.Wpf.Models.View;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace PaulZero.WindowsRoutine.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for FallbackNotificationWindow.xaml
    /// </summary>
    public partial class FallbackNotificationWindow : Window, IDisposable
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

        public async Task WaitForCompletionAsync()
        {
            await ViewModel.WaitForCompletionAsync();

            Close();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.SkipTimer();
        }

        public void Dispose()
        {
            ViewModel?.Dispose();
        }
    }
}
