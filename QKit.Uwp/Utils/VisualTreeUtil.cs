using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace QKit.Uwp.Utils
{
    public static class VisualTreeUtil
    {
        public static T FindParent<T>(this DependencyObject element)
             where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
                return null;

            return parent is T
                ? parent as T
                : parent.FindParent<T>();
        }
    }
}
