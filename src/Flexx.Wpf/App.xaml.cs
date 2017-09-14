using System.Windows;
using System.Windows.Threading;

namespace Flexx.Wpf
{
    public partial class App
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Ein Fehler ist aufgetreten: " + e?.Exception?.Message);
        }
    }
}