using System;
using System.Windows.Input;

namespace Flexx.Wpf
{
    /// <summary>
    /// Represents a <see cref="ICommand"/> implementation, that can be used vi delegates and lamda expressions.
    /// <remarks>Based on: https://wpftutorial.net/DelegateCommand.html</remarks>
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// Backup field for the <see cref="ICommand.CanExecute"/> <see cref="Predicate{T}"/>.
        /// </summary>
        private readonly Predicate<object> _canExecute;

        /// <summary>+
        /// Backup field for the <see cref="ICommand.Execute"/> <see cref="Action{T}"/>.
        /// </summary>
        private readonly Action<object> _execute;

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
        public DelegateCommand(Action<object> execute,
            Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Executes the <see cref="ICommand.CanExecute"/> <see cref="Predicate{T}"/> and returns it's result.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>True, if the command is able to execute.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Executes the <see cref="ICommand.Execute"/> <see cref="Action{T}"/> with the given parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}