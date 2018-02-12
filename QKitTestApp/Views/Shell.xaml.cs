using QDev.QKit.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Template10.Services.NavigationService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QKitTestApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static NavigationMenuView NavigationMenu => Instance.NavigationMenuRoot;

        private INavigationService _navigationService;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            //MyHamburgerMenu.NavigationService = navigationService;
            RemoveNavigationService();
            _navigationService = navigationService;
            _navigationService.AfterRestoreSavedNavigation += NavigationService_AfterRestoreSavedNavigation;
            _navigationService.FrameFacade.Navigated += FrameFacade_Navigated;
            NavigationMenuRoot.NavigationFrame = _navigationService.Frame;
        }

        public void RemoveNavigationService()
        {
            if (_navigationService == null)
                return;

            _navigationService.AfterRestoreSavedNavigation -= NavigationService_AfterRestoreSavedNavigation;
            _navigationService.FrameFacade.Navigated -= FrameFacade_Navigated;
            NavigationMenuRoot.NavigationFrame = null;
            _navigationService = null;
        }

        internal void HighlightCurrentMenuItem(Type pageType, object pageParam)
        {
            // match type only
            var menuItems = NavigationMenuRoot.PrimaryMenuItems
                .Union(NavigationMenuRoot.SecondaryMenuItems)
                .Where(x => Equals(x.PageType, pageType));

            // serialize parameter for matching
            if (pageParam == null)
            {
                pageParam = _navigationService.CurrentPageParam;
            }
            else if (pageParam.ToString().StartsWith("{"))
            {
                try
                {
                    pageParam = ((NavigationService)_navigationService).SerializationService.Deserialize(pageParam.ToString());
                }
                catch { }
            }

            // add parameter match
            menuItems = menuItems.Where(x => Equals(x.PageParameters, null) || Equals(x.PageParameters, pageParam));
            var menuItem = menuItems.Select(x => x).FirstOrDefault();
            if (menuItem != null)
                menuItem.IsChecked = true;
        }

        private void NavigationService_AfterRestoreSavedNavigation(object sender, Type e)
        {
            // _navigationService.CurrentPageType and CurrentPageParam is broken and only returns null. Workaround below.
            var savedNavigationServiceState = _navigationService.FrameFacade.PageStateSettingsService(_navigationService.GetType().ToString());
            var currentPageType = Type.GetType(savedNavigationServiceState.Read<string>("CurrentPageType"));
            var currentPageParam = savedNavigationServiceState.Read<object>("CurrentPageParam");

            HighlightCurrentMenuItem(currentPageType, currentPageParam);
        }

        private void FrameFacade_Navigated(object sender, NavigatedEventArgs e)
        {
            HighlightCurrentMenuItem(e.PageType, e.Parameter);
        }

        private void NavigationMenuRoot_SelectedMenuItemChanged(object sender, RoutedEventArgs e)
        {
            if (NavigationMenuRoot.SelectedMenuItem == null ||
                NavigationMenuRoot.SelectedMenuItem.PageType == null)
                return;

            _navigationService.Navigate(NavigationMenuRoot.SelectedMenuItem.PageType,
              NavigationMenuRoot.SelectedMenuItem.PageParameters);

            if (NavigationMenuRoot.SelectedMenuItem.ClearHistory)
                _navigationService.ClearHistory();
        }
    }
}
