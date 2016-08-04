using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace QKit.Controls
{
    public class NavigationMenuMainButton : ToggleButton
    {
        #region DependencyProperties
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(IconElement),
            typeof(NavigationMenuMainButton),
            new PropertyMetadata(null));
        #endregion

        #region Constructors
        public NavigationMenuMainButton()
        {
            DefaultStyleKey = typeof(NavigationMenuMainButton);
        }
        #endregion

        #region Properties
        public IconElement Icon
        {
            get { return (IconElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        #endregion
    }
}
