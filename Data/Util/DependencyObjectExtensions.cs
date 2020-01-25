using System.Windows;
using System.Windows.Media;

namespace InitiativeTracker.Data.Util
{
    /// <summary>
    /// Visual/logical tree extension functions. used to implement drag n drop functionality
    /// in the TreeView (entity tray). 
    /// </summary>
    public static class DependencyObjectExtensions
    {
        public static T FindVisualParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDependencyObject = child;
            do
            {
                parentDependencyObject = VisualTreeHelper.GetParent(parentDependencyObject);
                if (parentDependencyObject is T parent) return parent;
            }
            while (parentDependencyObject != null);
            return null;
        }

        public static T FindLogicalParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDependencyObject = child;
            do
            {
                parentDependencyObject = LogicalTreeHelper.GetParent(parentDependencyObject);
                if (parentDependencyObject is T parent) return parent;
            }
            while (parentDependencyObject != null);
            return null;
        }
    }
}
