using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QDev.QKit.Controls
{
    public class NavigationMenuItem : RadioButton
    {
        #region DependencyProperties
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(IconElement),
            typeof(NavigationMenuItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PageTypeProperty = DependencyProperty.Register(
            nameof(PageType),
            typeof(Type),
            typeof(NavigationMenuItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PageParametersProperty = DependencyProperty.Register(
            nameof(PageParameters),
            typeof(object),
            typeof(NavigationMenuItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ClearHistoryProperty = DependencyProperty.Register(
            nameof(ClearHistory),
            typeof(bool),
            typeof(NavigationMenuItem),
            new PropertyMetadata(default(bool)));
        #endregion

        #region Constructors
        public NavigationMenuItem()
        {
            DefaultStyleKey = typeof(NavigationMenuItem);
        }
        #endregion

        #region Properties
        public IconElement Icon
        {
            get { return (IconElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Type PageType
        {
            get { return (Type)GetValue(PageTypeProperty); }
            set { SetValue(PageTypeProperty, value); }
        }

        public object PageParameters
        {
            get { return GetValue(PageParametersProperty); }
            set { SetValue(PageParametersProperty, value); }
        }

        public bool ClearHistory
        {
            get { return (bool)GetValue(ClearHistoryProperty); }
            set { SetValue(ClearHistoryProperty, value); }
        }
        #endregion
    }
}
