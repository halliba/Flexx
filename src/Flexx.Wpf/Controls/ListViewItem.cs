using System.Windows.Input;

namespace Flexx.Wpf.Controls
{
    public class ListViewItem : System.Windows.Controls.ListViewItem
    {
        private bool _deferSelection;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 && IsSelected)
                _deferSelection = true;
            else
                base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_deferSelection)
            {
                try
                {
                    base.OnMouseLeftButtonDown(e);
                }
                finally
                {
                    _deferSelection = false;
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            _deferSelection = false;
            base.OnMouseLeave(e);
        }
    }
}