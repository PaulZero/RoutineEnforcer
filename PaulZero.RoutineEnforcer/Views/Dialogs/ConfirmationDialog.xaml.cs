using PaulZero.RoutineEnforcer.Views.Models.Dialogs;
using System.Windows;

namespace PaulZero.RoutineEnforcer.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : Window
    {
        public ConfirmationDialogViewModel ViewModel
        {
            get => DataContext as ConfirmationDialogViewModel;
            set => DataContext = value;
        }

        public ConfirmationDialog() : this(new ConfirmationDialogViewModel())
        {
        }

        public ConfirmationDialog(ConfirmationDialogViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            ViewModel.DialogResultChanged += ViewModel_DialogResultChanged;
        }

        private void ViewModel_DialogResultChanged(bool dialogResult)
        {
            DialogResult = dialogResult;
        }

        public static bool ShowYesNoDialog(string title, string message)
        {
            var dialog = new ConfirmationDialog(new ConfirmationDialogViewModel(title, message, "Yes", "No"));
            var result = dialog.ShowDialog();

            return result == true;
        }

        public static bool ShowOkCancelDialog(string title, string message)
        {
            var dialog = new ConfirmationDialog(new ConfirmationDialogViewModel(title, message));
            var result = dialog.ShowDialog();

            return result == true;
        }
    }
}
