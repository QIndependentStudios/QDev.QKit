using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QKitBlankApp.Feature.Grouping
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupingView : Page
    {
        private const double ImplicitAnimationDuration = 4;

        public GroupingView()
        {
            this.InitializeComponent();
            ConfigureImplicitAnimations();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= SystemNavigationManager_BackRequested;
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame.GoBack();
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void ConfigureImplicitAnimations()
        {
            var _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var fadeOut = _compositor.CreateScalarKeyFrameAnimation();
            fadeOut.Target = "Opacity";
            fadeOut.InsertKeyFrame(1, 0);
            fadeOut.Duration = TimeSpan.FromSeconds(ImplicitAnimationDuration);
            var fadeIn = _compositor.CreateScalarKeyFrameAnimation();
            fadeIn.Target = "Opacity";
            fadeIn.InsertKeyFrame(0, 0);
            fadeIn.InsertKeyFrame(1, 1);
            fadeIn.Duration = TimeSpan.FromSeconds(ImplicitAnimationDuration);

            ElementCompositionPreview.SetImplicitHideAnimation(ContentPanel, fadeOut);
            ElementCompositionPreview.SetImplicitShowAnimation(ContentPanel, fadeIn);
        }
    }
}
