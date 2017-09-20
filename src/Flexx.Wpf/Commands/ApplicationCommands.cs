using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flexx.Wpf.Commands
{
    internal class ChatAppCommands
    {
        public static readonly RoutedUICommand Leave = new RoutedUICommand("Leave", "Leave", typeof(ChatAppCommands));
    }
}
