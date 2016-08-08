using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QKit.Controls
{
    public class NavigationMenuMainButton : Button
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
            Click += NavigationMenuMainButton_Click;
            Unloaded += NavigationMenuMainButton_Unloaded;
        }
        #endregion

        #region Properties
        public IconElement Icon
        {
            get { return (IconElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        #endregion

        #region Event Handlers
        private void NavigationMenuMainButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var menu in NavigationMenuView.Instances)
            {
                menu.IsMenuOpen = !menu.IsMenuOpen;
            }
        }

        private void NavigationMenuMainButton_Unloaded(object sender, RoutedEventArgs e)
        {
            Click -= NavigationMenuMainButton_Click;
            Unloaded -= NavigationMenuMainButton_Unloaded;
        }
        #endregion
    }
}
