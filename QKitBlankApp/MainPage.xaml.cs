using QKitBlankApp.Feature.Grouping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QKitBlankApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const double ImplicitAnimationDuration = 4;

        public MainPage()
        {
            this.InitializeComponent();
            ConfigureImplicitAnimations();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ApplyAcrylicAccent(RootPanel);
            RootPanel.SizeChanged += RootPanel_SizeChanged;
        }

        private void RootPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _hostSprite.Size = new Vector2((float)(sender as Panel).ActualWidth, (float)(sender as Panel).ActualHeight);
        }

        private void ApplyAcrylicAccent(Panel panel)
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _hostSprite = _compositor.CreateSpriteVisual();
            _hostSprite.Size = new Vector2((float)panel.ActualWidth, (float)panel.ActualHeight);

            ElementCompositionPreview.SetElementChildVisual(panel, _hostSprite);
            _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
        }
        Compositor _compositor;
        SpriteVisual _hostSprite;

        private void GroupTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GroupingView));
        }

        private void BackButtonToggle_Checked(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((sender as ToggleButton)?.IsChecked ?? false)
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        private async void CustomTitle_BackRequested(object sender, EventArgs e)
        {
            await new MessageDialog("back requested").ShowAsync();
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
