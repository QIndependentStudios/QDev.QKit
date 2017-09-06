using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace QKit.Controls
{
    [TemplatePart(Name = CommonStatesName, Type = typeof(VisualStateGroup))]
    [ContentProperty(Name = nameof(DetailsView))]
    public sealed class MasterDetailsView : Control
    {
        #region Events
        public delegate void ViewStateChangingEventHandler(object sender, RoutedEventArgs e);
        public delegate void ViewStateChangedEventHandler(object sender, RoutedEventArgs e);
        public event ViewStateChangingEventHandler ViewStateChanging;
        public event ViewStateChangedEventHandler ViewStateChanged;
        #endregion

        #region Constants
        public const string CommonStatesName = "CommonStates";
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty NormalLayoutWidthThresholdProperty = DependencyProperty.Register(
            nameof(NormalLayoutWidthThreshold),
            typeof(double),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MasterPaneWidthProperty = DependencyProperty.Register(
            nameof(MasterPaneWidth),
            typeof(double),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MasterViewProperty = DependencyProperty.Register(
            nameof(MasterView),
            typeof(object),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty DetailsViewProperty = DependencyProperty.Register(
            nameof(DetailsView),
            typeof(object),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty IsStackedModeProperty = DependencyProperty.Register(
            nameof(IsStackedMode),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsDetailsViewInStackedModeProperty = DependencyProperty.Register(
            nameof(IsDetailsViewInStackedMode),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(bool),
                (sender, args) =>
                {
                    var control = sender as MasterDetailsView;

                    if (control == null)
                        return;

                    control.UpdateViewState();
                }));

        public static readonly DependencyProperty CanExitDetailsViewProperty = DependencyProperty.Register(
            nameof(CanExitDetailsView),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register(
            nameof(IsAnimated),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(true));
        #endregion

        #region Template Parts
        private VisualStateGroup CommonStates { get; set; }
        #endregion

        #region Properties
        public double NormalLayoutWidthThreshold
        {
            get { return (double)GetValue(NormalLayoutWidthThresholdProperty); }
            set { SetValue(NormalLayoutWidthThresholdProperty, value); }
        }

        public double MasterPaneWidth
        {
            get { return (double)GetValue(MasterPaneWidthProperty); }
            set { SetValue(MasterPaneWidthProperty, value); }
        }

        public object MasterView
        {
            get { return GetValue(MasterViewProperty); }
            set { SetValue(MasterViewProperty, value); }
        }

        public object DetailsView
        {
            get { return GetValue(DetailsViewProperty); }
            set { SetValue(DetailsViewProperty, value); }
        }

        public bool IsStackedMode
        {
            get { return (bool)GetValue(IsStackedModeProperty); }
            private set { SetValue(IsStackedModeProperty, value); }

        }

        public bool IsDetailsViewInStackedMode
        {
            get { return (bool)GetValue(IsDetailsViewInStackedModeProperty); }
            set { SetValue(IsDetailsViewInStackedModeProperty, value); }
        }

        public bool CanExitDetailsView
        {
            get { return (bool)GetValue(CanExitDetailsViewProperty); }
            private set { SetValue(CanExitDetailsViewProperty, value); }

        }

        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }
        #endregion

        #region Constructors
        public MasterDetailsView()
        {
            DefaultStyleKey = typeof(MasterDetailsView);
        }
        #endregion

        #region Methods
        private static Storyboard CreateFallbackAnimation(DependencyObject entranceTarget, DependencyObject exitTarget)
        {
            var sb = new Storyboard();
            var fadeIn = new FadeInThemeAnimation();
            var fadeOut = new FadeOutThemeAnimation();

            Storyboard.SetTarget(fadeIn, entranceTarget);
            Storyboard.SetTarget(fadeOut, exitTarget);
            sb.Children.Add(fadeIn);
            sb.Children.Add(fadeOut);
            return sb;
        }

        protected override void OnApplyTemplate()
        {
            if (CommonStates != null)
                CommonStates.CurrentStateChanged -= CommonStates_CurrentStateChanged;

            CommonStates = GetTemplateChild(CommonStatesName) as VisualStateGroup;

            if (CommonStates != null)
                CommonStates.CurrentStateChanged += CommonStates_CurrentStateChanged;

            SizeChanged += MasterDetailsView_SizeChanged;
        }

        private void UpdateViewState()
        {
            if (ActualWidth < NormalLayoutWidthThreshold)
                VisualStateManager.GoToState(this,
                    IsDetailsViewInStackedMode ? "DetailsVisualState" : "MasterVisualState",
                    IsAnimated);
            else
                VisualStateManager.GoToState(this, "NormalVisualState", false);

            IsStackedMode = ActualWidth < NormalLayoutWidthThreshold;
            CanExitDetailsView = IsDetailsViewInStackedMode && IsStackedMode;

            OnViewStateChanging();
        }

        private void OnViewStateChanging()
        {
            if (ViewStateChanging == null)
                return;

            var args = new RoutedEventArgs();
            ViewStateChanging(this, args);
        }

        private void OnViewStateChanged()
        {
            if (ViewStateChanged == null)
                return;

            var args = new RoutedEventArgs();
            ViewStateChanged(this, args);
        }
        #endregion

        #region Event Handlers
        private void CommonStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            OnViewStateChanged();
        }

        private void MasterDetailsView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateViewState();
        }
        #endregion
    }
}
