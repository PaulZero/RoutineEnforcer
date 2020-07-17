using PaulZero.WindowsRoutine.Wpf.Models.View;
using System;
using System.Windows;

namespace PaulZero.WindowsRoutine.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindowViewModel ViewModel
        {
            get => DataContext as ErrorWindowViewModel;
            set => DataContext = value;
        }

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow(string title, string message, Exception exception) : this()
        {
            ViewModel = new ErrorWindowViewModel(title, message, exception);
        }

        public static bool TryShow(string title, string message, Exception exception)
        {
            try
            {
                var window = new ErrorWindow(title, message, exception);

                window.Show();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryShowDialog(string title, string message, Exception exception)
        {
            try
            {
                var window = new ErrorWindow(title, message, exception);

                return window.ShowDialog() ?? false;
            }
            catch
            {
                return false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ViewModel.CreateClipboardSummary());
        }
    }
}
