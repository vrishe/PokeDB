using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Linq;

namespace PokeDB.Infrastructure
{
    public class RelayCommand<T> : ICommand
    {
        readonly Action<T> execute;
        readonly Func<T, bool> canExecute;


        public RelayCommand(Action<T> execute,
                    Func<T, bool> canExecute,
                    INotifyPropertyChanged notifyPropertyChanged,
                    params string[] properties)

            : this(execute, canExecute, notifyPropertyChanged,
                   propertyChanged => properties.Any(property => property == propertyChanged))
        {
            /* Nothing to do */
        }

        public RelayCommand(Action<T> execute,
                            Func<T, bool> canExecute,
                            INotifyPropertyChanged notifyPropertyChanged,
                            Func<string, bool> propertyFilter = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            notifyPropertyChanged.PropertyChanged += (sender, e) =>
            {
                if (propertyFilter == null || propertyFilter(e.PropertyName))
                {
                    CanExecuteChanged?.Invoke(sender, EventArgs.Empty);
                }
            };
        }


        T GetAsTarget(object parameter)
        {
            try
            {
                return ((T)parameter);
            }
            catch (InvalidCastException e)
            {
                throw new Exception(string.Format(
                    "This command expects parameter of type {0}, but {1} is given.",
                    typeof(T), parameter != null ? parameter.GetType().ToString() : "null"
                ), e);
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(GetAsTarget(parameter));
        }

        public void Execute(object parameter)
        {
            execute(GetAsTarget(parameter));
        }


        public event EventHandler CanExecuteChanged;

    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute,
            Func<bool> canExecute,
            INotifyPropertyChanged notifyPropertyChanged,
            params string[] properties)

            : base(p => execute(), canExecute != null ?
                 new Func<object, bool>(p => canExecute()) : null, notifyPropertyChanged, properties)
        {
            /* Nothing to do */
        }

        public RelayCommand(Action execute,
                    Func<bool> canExecute,
                    INotifyPropertyChanged notifyPropertyChanged,
                    Func<string, bool> propertyFilter = null)

            : base(p => execute(), canExecute != null ?
                 new Func<object, bool>(p => canExecute()) : null, notifyPropertyChanged, propertyFilter)
        {
            /* Nothing to do */
        }

        public RelayCommand(Action<object> execute,
                            Func<object, bool> canExecute,
                            INotifyPropertyChanged notifyPropertyChanged,
                            params string[] properties)

            : base(execute, canExecute, notifyPropertyChanged, properties)
        {
            /* Nothing to do */
        }

        public RelayCommand(Action<object> execute,
                            Func<object, bool> canExecute,
                            INotifyPropertyChanged notifyPropertyChanged,
                            Func<string, bool> propertyFilter = null)

            : base(execute, canExecute, notifyPropertyChanged, propertyFilter)
        {
            /* Nothing to do */
        }
    }
}

