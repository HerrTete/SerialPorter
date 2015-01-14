using System;
using System.Windows.Input;

namespace SerialPorter.WpfTools
{
    public class BaseCommand : ICommand
    {
        private readonly Action<object> _executeActionWithParameter;
        private readonly Func<object, bool> _canExecuteFuncWithParameter;
        
        private readonly Action _executeAction;
        private readonly Func<bool> _canExecuteFunc;

        private readonly bool _parameterless = false;

        public BaseCommand(Action executeAction, Func<bool> canExecuteFunc = null)
        {
            _executeAction = executeAction;
            _canExecuteFunc = canExecuteFunc;
            _parameterless = true;
        }

        public BaseCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc = null)
        {
            _executeActionWithParameter = executeAction;
            _canExecuteFuncWithParameter = canExecuteFunc;
        }

        public bool CanExecute(object parameter)
        {
            var canExecute = false;
            if (_canExecuteFunc == null)
            {
                canExecute = true;
            }
            else
            {
                canExecute = _parameterless ? _canExecuteFunc() : _canExecuteFuncWithParameter(parameter);
            }

            return canExecute;
        }

        public void Execute(object parameter)
        {
            if (_parameterless)
            {
                _executeAction();
            }
            else
            {
                _executeActionWithParameter(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged !=null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
