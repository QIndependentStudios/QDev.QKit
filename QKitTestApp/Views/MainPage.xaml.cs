using QKit.Uwp.Controls;
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
            //if (MasterDetails.IsStackedMode && MasterDetails.ShowDetailsInStackedMode)
            //    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("back", Circle);
        }

        private object _clickedItem;

        private void MasterList_ItemClick(object sender, ItemClickEventArgs e)
        {
            _clickedItem = e.ClickedItem;
            MasterList.PrepareConnectedAnimation("x", e.ClickedItem, "Circle");
            ConnectedAnimationService.GetForCurrentView().GetAnimation("x").TryStart(Circle);
        }

        public async void MasterDetails_ViewStateChanging(object sender, MasterDetailsViewStateChangeEventArgs e)
        {
            var masterDetails = sender as MasterDetailsView;

            if (masterDetails == null)
                return;


            if (e.OldValue == MasterDetailsViewState.Details && e.NewValue == MasterDetailsViewState.Master && _clickedItem != null)
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("back", Circle);
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("back");
                await MasterList.TryStartConnectedAnimationAsync(ConnectedAnimationService.GetForCurrentView().GetAnimation("back"), _clickedItem, "Circle");
            }

            App.Current.ForceShowShellBackButton = e.NewValue == MasterDetailsViewState.Details;
            Template10.Common.BootStrapper.Current.UpdateShellBackButton();
        }
    }
}
