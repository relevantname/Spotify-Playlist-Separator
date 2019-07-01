using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpotifyExtension.Tools
{
    public class RelayCommand : ICommand
    {
        private Action<object> _executeMethod;
        private Func<bool, object> _canExecuteMethod;

        public RelayCommand(Action<object> executeMethod, Func<bool,object> canExecuteMethod)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteMethod != null)
                return true;
            else
                return false;
        }

        public event EventHandler CanExecuteChanged{
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }
    }
}
