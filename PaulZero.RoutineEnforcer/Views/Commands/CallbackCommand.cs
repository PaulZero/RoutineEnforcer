using System;
using System.Windows.Input;

namespace PaulZero.RoutineEnforcer.Views.Commands
{
    internal class CallbackCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        public CallbackCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public CallbackCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public void Refresh()
        {
            if (_canExecute is null)
            {
                return;
            }

            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
