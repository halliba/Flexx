using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Flexx.Wpf.Extensions
{
    internal static class DependencyObjectExtensions
    {
        internal static T FindAnchestor<T>(this DependencyObject child) where T : DependencyObject
        {
            do
            {
                if (child is T anchestor)
                    return anchestor;
                try
                {
                    child = VisualTreeHelper.GetParent(child);
                }
                catch (InvalidOperationException)
                {
                    child = null;
                }
            }
            while (child != null);
            return null;
        }

        /// <summary>
        /// Finds all Children of a <see cref="DependencyObject"/>
        /// </summary>
        /// <param name="parent">The parent node from where the tree will be searched</param>
        /// <returns>All found children</returns>
        public static IEnumerable<UIElement> GetAllChildren(this DependencyObject parent)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                // retrieve child at specified index
                var directChild = (Visual)VisualTreeHelper.GetChild(parent, i);

                var uiChildren = directChild as UIElement;
                if (uiChildren == null) continue;

                // return found child
                yield return uiChildren;

                // return all children of the found child
                foreach (var nestedChild in directChild.GetAllChildren())
                    yield return nestedChild;
            }
        }

        /// <summary>
        /// Finds all Children of a <see cref="DependencyObject"/> until it finds a child of type <typeparamref name="TUntil"/>.
        /// </summary>
        /// <typeparam name="TUntil">The node type until the tree will be searched</typeparam>
        /// <param name="parent">The parent node from where the tree will be searched</param>
        /// <returns>All found children</returns>
        public static IEnumerable<UIElement> GetAllChildrenUntil<TUntil>(this DependencyObject parent)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                // retrieve child at specified index
                var directChild = (Visual)VisualTreeHelper.GetChild(parent, i);

                var uiChildren = directChild as UIElement;
                if (uiChildren == null || uiChildren is TUntil) continue;

                // return found child
                yield return uiChildren;

                // return all children of the found child
                foreach (var nestedChild in directChild.GetAllChildrenUntil<TUntil>())
                    yield return nestedChild;
            }
        }
    }
}