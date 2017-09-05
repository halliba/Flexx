using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Flexx.Wpf.Controls
{
    public class CommandTextBox : TextBox
    {
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty CommandParameterProperty;
        public static readonly DependencyProperty ClearOnCommandSuccessProperty;

        private bool _returnDown;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public bool ClearOnCommandSuccess
        {
            get => (bool)GetValue(ClearOnCommandSuccessProperty);
            set => SetValue(ClearOnCommandSuccessProperty, value);
        }

        static CommandTextBox()
        {
            CommandProperty = DependencyProperty.Register("Command", typeof(ICommand),
                typeof(CommandTextBox), new FrameworkPropertyMetadata(null));
            CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object),
                typeof(CommandTextBox), new FrameworkPropertyMetadata(null));
            ClearOnCommandSuccessProperty = DependencyProperty.Register("ClearOnCommandSuccess", typeof(bool),
                typeof(CommandTextBox), new PropertyMetadata(true));
        }

        public CommandTextBox()
        {
            PreviewKeyDown += OnPreviewKeyDown;
            PreviewKeyUp += OnPreviewKeyUp;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Return
                && (AcceptsReturn && (Keyboard.Modifiers & ModifierKeys.Shift) != 0
                    || !AcceptsReturn))
            {
                _returnDown = true;
                keyEventArgs.Handled = true;
            }
            else
            {
                _returnDown = false;
            }
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != Key.Return || !_returnDown || Command == null) return;
            if (AcceptsReturn && (Keyboard.Modifiers & ModifierKeys.Shift) == 0) return;
            keyEventArgs.Handled = true;
            _returnDown = false;
            try
            {
                Command.Execute(CommandParameter);
                Clear();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
