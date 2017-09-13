using System;
using System.Windows.Input;
using Flexx.Wpf.Annotations;

namespace Flexx.Wpf
{
    public class DelegateCommand<T> : ICommand
    {
        /// <summary>
        /// Backup field for the <see cref="ICommand.CanExecute"/> <see cref="Predicate{T}"/>.
        /// </summary>
        private readonly Predicate<T> _canExecute;

        /// <summary>+
        /// Backup field for the <see cref="ICommand.Execute"/> <see cref="Action{T}"/>.
        /// </summary>
        private readonly Action<T> _execute;

        public bool AllowNull { get; }

        /// <summary>
        /// Gets executed, when the <see cref="CanExecute"/> result probably changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a new Instance of the <see cref="DelegateCommand"/> class with the given <see cref="ICommand.Execute"/> <see cref="Action{T}"/>
        /// and optionally a <see cref="ICommand.CanExecute"/> <see cref="Predicate{T}"/>.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/>, to be exeucted on <see cref="Execute"/>.</param>
        /// <param name="canExecute">The <see cref="Predicate{T}"/>, executed in <see cref="CanExecute"/> that indicates if the command
        /// is able to execute.</param>
        /// <param name="allowNull"></param>
        public DelegateCommand([NotNull] Action<T> execute, Predicate<T> canExecute = null, bool allowNull = false)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));

            _canExecute = canExecute;
            AllowNull = allowNull;
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Executes the <see cref="M:System.Windows.Input.ICommand.CanExecute(System.Object)" /> <see cref="T:System.Predicate`1" /> and returns it's result.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>True, if the command is able to execute.</returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            if (parameter is T value)
                return _canExecute(value);
            
            return AllowNull && parameter == null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Executes the <see cref="M:System.Windows.Input.ICommand.Execute(System.Object)" /> <see cref="T:System.Action`1" /> with the given parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public void Execute(object parameter)
        {
            if (parameter is T value)
                _execute(value);
            else if (AllowNull && parameter == null)
                _execute(default(T));
        }
    }
}