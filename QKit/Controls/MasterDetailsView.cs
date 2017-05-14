using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace QKit.Controls
{
    [TemplatePart(Name = MasterPresenterName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = DetailsPresenterName, Type = typeof(ContentPresenter))]
    [TemplateVisualState(Name = NormalVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [TemplateVisualState(Name = NarrowVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [ContentProperty(Name = nameof(DetailsView))]
    public sealed class MasterDetailsView : Control
    {
        #region Events
        public delegate void ViewStateChangedEventHandler(object sender, RoutedEventArgs e);
        public event ViewStateChangedEventHandler ViewStateChanged;
        #endregion

        #region Constants
        public const string MasterPresenterName = "MasterPresenter";
        public const string DetailsPresenterName = "DetailsPresenter";
        public const string AdaptiveVisualStateGroupName = "AdaptiveVisualStateGroup";
        public const string NormalVisualStateName = "NormalVisualState";
        public const string NarrowVisualStateName = "NarrowVisualState";
        public const string DetailsDrillInAnimationName = "DetailsDrillIn";
        public const string DetailsDrillOutAnimationName = "DetailsDrillOut";
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty NormalStateMinWidthProperty = DependencyProperty.Register(
            nameof(NormalStateMinWidth),
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

                    if (control == null || control.AdaptiveVisualStateGroup == null)
                        return;

                    // Not a visual state change, but still a logical state change.
                    // ex. VisualStateNarrow/MasterView -> VisualStateNarrow/DetailsView
                    control._previousState = control.AdaptiveVisualStateGroup.CurrentState;
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
        private ContentPresenter MasterPresenter { get; set; }
        private ContentPresenter DetailsPresenter { get; set; }

        private VisualStateGroup AdaptiveVisualStateGroup { get; set; }
        private VisualState NormalVisualState { get; set; }
        private VisualState NarrowVisualState { get; set; }

        private Storyboard DetailDrillIn { get; set; }
        private Storyboard DetailDrillOut { get; set; }
        #endregion

        #region Members
        private VisualState _previousState;
        #endregion

        #region Properties
        public double NormalStateMinWidth
        {
            get { return (double)GetValue(NormalStateMinWidthProperty); }
            set { SetValue(NormalStateMinWidthProperty, value); }
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
        protected override void OnApplyTemplate()
        {
            // Controls
            MasterPresenter = GetTemplateChild(MasterPresenterName) as ContentPresenter;
            DetailsPresenter = GetTemplateChild(DetailsPresenterName) as ContentPresenter;

            // Visual States
            AdaptiveVisualStateGroup = GetTemplateChild(AdaptiveVisualStateGroupName) as VisualStateGroup;
            NormalVisualState = GetTemplateChild(NormalVisualStateName) as VisualState;
            NarrowVisualState = GetTemplateChild(NarrowVisualStateName) as VisualState;

            if (AdaptiveVisualStateGroup != null)
                AdaptiveVisualStateGroup.CurrentStateChanged += AdaptiveVisualStateGroupElement_CurrentStateChanged;

            // Animations
            DetailDrillIn = GetTemplateChild(DetailsDrillInAnimationName) as Storyboard;
            DetailDrillOut = GetTemplateChild(DetailsDrillOutAnimationName) as Storyboard;
        }

        private void UpdateViewState()
        {
            var previousState = _previousState ?? NormalVisualState;
            if (previousState == NormalVisualState &&
                AdaptiveVisualStateGroup.CurrentState == NarrowVisualState)
            {
                if (IsDetailsViewInStackedMode)
                {
                    // normal -> master
                    DetailDrillIn.Begin();
                    DetailDrillIn.SkipToFill();
                    SetHitTestVisibility(MasterPresenter, false);
                    SetHitTestVisibility(DetailsPresenter, true);
                }
                else
                {
                    // normal -> details
                    DetailDrillOut.Begin();
                    DetailDrillOut.SkipToFill();
                    SetHitTestVisibility(MasterPresenter, true);
                    SetHitTestVisibility(DetailsPresenter, false);
                }

                UpdateReadonlyStateProperties();
                OnViewStateChanged();
            }
            else if (previousState == NarrowVisualState &&
                AdaptiveVisualStateGroup.CurrentState == NormalVisualState)
            {
                // master -> normal
                // details -> normal
                ResetViewForNormal();
                UpdateReadonlyStateProperties();
                OnViewStateChanged();
            }
            else if (previousState == NarrowVisualState &&
                AdaptiveVisualStateGroup.CurrentState == NarrowVisualState)
            {
                if (IsDetailsViewInStackedMode)
                {
                    // master -> details
                    DetailDrillIn.Begin();

                    if (!IsAnimated)
                        DetailDrillIn.SkipToFill();

                    SetHitTestVisibility(MasterPresenter, false);
                    SetHitTestVisibility(DetailsPresenter, true);
                }
                else
                {
                    // details -> master
                    DetailDrillOut.Begin();

                    if (!IsAnimated)
                        DetailDrillOut.SkipToFill();

                    SetHitTestVisibility(MasterPresenter, true);
                    SetHitTestVisibility(DetailsPresenter, false);
                }

                UpdateReadonlyStateProperties();
                OnViewStateChanged();
            }

            UpdateReadonlyStateProperties();
        }

        private void UpdateReadonlyStateProperties()
        {
            if (AdaptiveVisualStateGroup == null)
                return;

            IsStackedMode = AdaptiveVisualStateGroup.CurrentState == NarrowVisualState;

            CanExitDetailsView = IsDetailsViewInStackedMode &&
                AdaptiveVisualStateGroup.CurrentState == NarrowVisualState;
        }

        private void OnViewStateChanged()
        {
            if (ViewStateChanged == null)
                return;

            var args = new RoutedEventArgs();
            ViewStateChanged(this, args);
        }

        private void ResetViewForNormal()
        {
            DetailDrillIn?.Stop();
            DetailDrillOut?.Stop();

            SetHitTestVisibility(MasterPresenter, true);
            SetHitTestVisibility(DetailsPresenter, true);
        }

        private void SetHitTestVisibility(FrameworkElement element, bool isHitTestVisible)
        {
            if (element != null)
                element.IsHitTestVisible = isHitTestVisible;
        }
        #endregion

        #region Event Handlers
        private void AdaptiveVisualStateGroupElement_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            _previousState = e.OldState;
            UpdateViewState();
        }
        #endregion
    }
}
