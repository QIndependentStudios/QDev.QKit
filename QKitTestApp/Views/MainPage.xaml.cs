using QKit.Controls;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace QKitTestApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            App.BackRequested += App_BackRequested;
        }

        private void App_BackRequested(object sender, Template10.Common.HandledEventArgs e)
        {
            //if (MasterDetails.IsStackedMode && MasterDetails.IsDetailsViewInStackedMode)
            //    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("back", Circle);
        }

        private object _clickedItem;

        private void MasterList_ItemClick(object sender, ItemClickEventArgs e)
        {
            _clickedItem = e.ClickedItem;
            MasterList.PrepareConnectedAnimation("x", e.ClickedItem, "Circle");
            ConnectedAnimationService.GetForCurrentView().GetAnimation("x").TryStart(Circle);
        }

        public async void MasterDetails_ViewStateChanging(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var masterDetails = sender as MasterDetailsView;

            if (masterDetails == null)
                return;


            if (masterDetails.IsStackedMode && !masterDetails.IsDetailsViewInStackedMode && _clickedItem != null)
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("back", Circle);
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("back");
                await MasterList.TryStartConnectedAnimationAsync(ConnectedAnimationService.GetForCurrentView().GetAnimation("back"), _clickedItem, "Circle");
            }

            App.Current.ForceShowShellBackButton = masterDetails.CanExitDetailsView;
            Template10.Common.BootStrapper.Current.UpdateShellBackButton();
        }
    }
}
