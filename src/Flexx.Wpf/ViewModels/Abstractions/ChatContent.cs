using System;

namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IChatContent
    {
        DateTime TimeStamp { get; }

        bool IsUnread { get; set; }
    }
}