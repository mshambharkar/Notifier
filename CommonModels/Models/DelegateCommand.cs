using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CommonModels.Models
{
   
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _commandWithParameter;
        private readonly Action _command;
        private Func<bool> _commandBoolRet;
        private Func<bool> _canExecute;
        private readonly Func<object, bool> _canExecuteWithParameter;

        public DelegateCommand()
        {

        }

        public DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            _canExecute = canExecute;
            _command = command;
        }


        public DelegateCommand(Action<object> commandWithParameter, Func<object, bool> canExecuteWithParameter = null)
        {
            if (commandWithParameter == null)
                throw new ArgumentNullException("commandWithParameter");
            _canExecuteWithParameter = canExecuteWithParameter;
            _commandWithParameter = commandWithParameter;
        }

        public void SetDelegateCommand(Func<bool> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            _canExecute = canExecute;
            _commandBoolRet = command;
        }

        public void Execute(object parameter)
        {
            if (_command != null)
                _command();
            if (_commandWithParameter != null)
                _commandWithParameter(parameter);
            if (_commandBoolRet != null)
                _commandBoolRet();
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            if (_canExecuteWithParameter != null)
                return _canExecuteWithParameter(parameter);

            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

}
