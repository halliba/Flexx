using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Flexx.Wpf.Extensions;

namespace Flexx.Wpf.Controls
{
    internal class DragDropListView : System.Windows.Controls.ListView
    {
        private Point _dragStartPoint;
        public static readonly DependencyProperty AllowItemDragProperty;
        public static readonly DependencyProperty AllowItemDropProperty;
        public static readonly DependencyProperty DropDataFormatProperty;
        public static readonly DependencyProperty DragDataFormatProperty;
        public static readonly DependencyProperty DropDataFormatsProperty;
        public static readonly DependencyProperty DropCommandProperty;
        
        public bool AllowItemDrag
        {
            set => SetValue(AllowItemDragProperty, value);
            get => (bool)GetValue(AllowItemDragProperty);
        }
        
        public bool AllowItemDrop
        {
            set => SetValue(AllowItemDropProperty, value);
            get => (bool)GetValue(AllowItemDropProperty);
        }
        
        public string DragDataFormat
        {
            set => SetValue(DragDataFormatProperty, value);
            get => (string)GetValue(DragDataFormatProperty);
        }
        
        public string DropDataFormat
        {
            set => SetValue(DropDataFormatProperty, value);
            get => (string)GetValue(DropDataFormatProperty);
        }
        
        public DropDataFormatCollection DropDataFormats
        {
            set => SetValue(DropDataFormatsProperty, value);
            get => (DropDataFormatCollection)GetValue(DropDataFormatsProperty);
        }
        
        public ICommand DropCommand
        {
            set => SetValue(DropCommandProperty, value);
            get => (ICommand)GetValue(DropCommandProperty);
        }

        static DragDropListView()
        {
            AllowItemDragProperty = DependencyProperty.Register("AllowItemDrag",
                typeof(bool), typeof(DragDropListView), new FrameworkPropertyMetadata(false));

            AllowItemDropProperty = DependencyProperty.Register("AllowItemDrop",
                typeof(bool), typeof(DragDropListView), new FrameworkPropertyMetadata(false));

            DragDataFormatProperty = DependencyProperty.Register("DragDataFormat",
                typeof(string), typeof(DragDropListView), new FrameworkPropertyMetadata(null));

            DropDataFormatProperty = DependencyProperty.Register("DropDataFormat",
                typeof(string), typeof(DragDropListView), new FrameworkPropertyMetadata(null));

            DropDataFormatsProperty = DependencyProperty.Register("DropDataFormats",
                typeof(DropDataFormatCollection), typeof(DragDropListView), new FrameworkPropertyMetadata(null));

            DropCommandProperty = DependencyProperty.Register("DropCommand",
                typeof(ICommand), typeof(DragDropListView), new FrameworkPropertyMetadata(null));
        }

        public DragDropListView()
        {
            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            PreviewMouseMove += OnPreviewMouseMove;
            DragEnter += OnDragEnter;
            Drop += OnDrop;
        }

        #region multipleDragFix

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListViewItem();
        }

        #endregion

        #region drag

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!AllowItemDrag) return;
            _dragStartPoint = e.GetPosition(null);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!AllowItemDrag) return;
            if (string.IsNullOrEmpty(DragDataFormat))
                throw new InvalidOperationException($"{nameof(DragDataFormat)} not set.");

            var mousePos = e.GetPosition(null);
            var diff = _dragStartPoint - mousePos;

            if (e.LeftButton != MouseButtonState.Pressed ||
                Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance) return;

            var listView = sender as DragDropListView;
            var items = listView?.SelectedItems.OfType<object>().ToList();
            if (items == null || items.Count == 0) return;

            var listViewItem = ((DependencyObject)e.OriginalSource).FindAnchestor<ListViewItem>();
            if (listViewItem == null) return;

            var dragData = new DataObject(DragDataFormat, items);
            DragDrop.DoDragDrop(listView, dragData, DragDropEffects.Move);
        }

        #endregion

        #region drop

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (!AllowDrop || !AllowItemDrop) return;

            if (!e.Data.GetDataPresent(DropDataFormat) || sender == e.Source)
                e.Effects = DragDropEffects.None;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!AllowDrop || !AllowItemDrop || DropCommand == null) return;
            if (string.IsNullOrEmpty(DropDataFormat) && (DropDataFormats == null || DropDataFormats.Count == 0))
                throw new InvalidOperationException($"{nameof(DropDataFormat)} not set and {nameof(DropDataFormats)} is null or empty.");

            if (!e.Data.GetDataPresent(DropDataFormat)) return;
            if (!(e.Data.GetData(DropDataFormat) is IEnumerable<object> items)) return;
            DropCommand.Execute(items);
        }

        #endregion
    }
}