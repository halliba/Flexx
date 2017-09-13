using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Flexx.Wpf
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void MessageBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Ein Fehler ist aufgetreten: " + e?.Exception?.Message);
        }
    }
}
