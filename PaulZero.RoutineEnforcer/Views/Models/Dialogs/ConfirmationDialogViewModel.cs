using PaulZero.RoutineEnforcer.Views.Commands;
using System;
using System.Windows.Input;

namespace PaulZero.RoutineEnforcer.Views.Models.Dialogs
{
    public class ConfirmationDialogViewModel
    {
        public event Action<bool> DialogResultChanged;

        public string TitleText { get; }

        public string Message { get; }

        public string OkText { get; }

        public string CancelText { get; }

        public ICommand CancelCommand { get; }

        public ICommand OkCommand { get; }

        public ConfirmationDialogViewModel()
            : this("Please Confirm", "Confirm whether you wish to do the thing.")
        {
        }

        public ConfirmationDialogViewModel(string titleText, string message, string okText = "OK", string cancelText = "Cancel")
        {
            TitleText = titleText;
            Message = message;
            OkText = okText;
            CancelText = cancelText;

            CancelCommand = new CallbackCommand(Cancel);
            OkCommand = new CallbackCommand(Ok);
        }

        private void Ok(object parameter = null)
        {
            DialogResultChanged?.Invoke(true);
        }

        private void Cancel(object parameter = null)
        {
            DialogResultChanged?.Invoke(false);
        }
    }
}
