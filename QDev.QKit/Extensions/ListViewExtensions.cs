using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace QDev.QKit.Extensions
{
    public class ListViewExtensions
    {
        public static readonly DependencyProperty ScrollToSelectedItemProperty =
            DependencyProperty.RegisterAttached("ScrollToSelectedItem",
                typeof(bool),
                typeof(ListViewExtensions),
                new PropertyMetadata(false, ScrollToSelectedItemChanged));

        public static bool GetScrollToSelectedItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollToSelectedItemProperty);
        }

        public static void SetScrollToSelectedItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollToSelectedItemProperty, value);
        }

        private static void ScrollToSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var listViewBase = obj as ListViewBase;

            if (listViewBase == null)
                return;


            listViewBase.SelectionChanged -= ListViewBase_SelectionChanged;

            if ((bool)e.NewValue)
                listViewBase.SelectionChanged += ListViewBase_SelectionChanged;
        }

        private static void ListViewBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listViewBase = sender as ListViewBase;
            var added = e.AddedItems.DefaultIfEmpty(null).FirstOrDefault();

            if (listViewBase == null || added == null)
                return;

            listViewBase.UpdateLayout();

            var semanticZoom = VisualTreeHelper.GetParent(listViewBase) as SemanticZoom;

            if (semanticZoom == null)
                listViewBase.ScrollIntoView(added);
            else
            {
                var loc = new SemanticZoomLocation { Item = added };
                listViewBase.MakeVisible(loc);
            }
        }
    }
}
