using System;
using System.Windows.Input;

namespace FluidIrc.ViewModels
{
    public class RelayCommand : ICommand
    {

        private readonly Action _action;
        private readonly bool _canExecute;

        public RelayCommand(Action action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
