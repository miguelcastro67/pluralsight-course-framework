using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Core.Common.UI.Core
{
    public class DelegateCommand<T> : ICommand
    {
        public DelegateCommand(Action<T> execute) : this(execute, null) { }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute) : this(execute, canExecute, "") { }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute, string label)
        {
            _Execute = execute;
            _CanExecute = canExecute;

            Label = label;
        }

        readonly Action<T> _Execute = null;
        readonly Predicate<T> _CanExecute = null;

        public string Label { get; set; }

        public void Execute(object parameter)
        {
            _Execute((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute == null ? true : _CanExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_CanExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_CanExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }
    }
}
