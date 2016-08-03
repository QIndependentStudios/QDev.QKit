using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace QKit.Controls
{
    [TemplatePart(Name = MasterPresenterName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = DetailsPresenterName, Type = typeof(ContentControl))]
    [TemplateVisualState(Name = NormalVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [TemplateVisualState(Name = NarrowVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [ContentProperty(Name = nameof(Content))]
    public sealed class MasterDetailsView : ContentControl
    {
        public delegate void ViewStateChangedEventHandler(object sender, RoutedEventArgs e);
        public event ViewStateChangedEventHandler ViewStateChanged;

        #region Constants
        public const string MasterPresenterName = "MasterPresenter";
        public const string DetailsPresenterName = "DetailsPresenter";
        public const string AdaptiveVisualStateGroupName = "AdaptiveVisualStateGroup";
        public const string NormalVisualStateName = "VisualStateNormal";
        public const string NarrowVisualStateName = "VisualStateNarrow";
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

        public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(
            nameof(DetailsTemplate),
            typeof(DataTemplate),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty DetailsProperty = DependencyProperty.Register(
            nameof(Details),
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

                    if (control == null || control.AdaptiveVisualStateGroupElement == null)
                        return;

                    // Not a visual state change, but still a logical state change.
                    // ex. VisualStateNarrow/MasterView -> VisualStateNarrow/DetailsView
                    control._previousState = control.AdaptiveVisualStateGroupElement.CurrentState;
                    control.UpdateViewState();
                }));

        public static readonly DependencyProperty CanExitDetailsViewProperty = DependencyProperty.Register(
            nameof(CanExitDetailsView),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(bool)));
        #endregion

        #region Template Parts
        private ContentPresenter MasterPresenter { get; set; }
        private ContentControl DetailsPresenter { get; set; }

        private VisualStateGroup AdaptiveVisualStateGroupElement { get; set; }
        private VisualState NormalVisualStateElement { get; set; }
        private VisualState NarrowVisualStateElement { get; set; }

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

        public DataTemplate DetailsTemplate
        {
            get { return (DataTemplate)GetValue(DetailsTemplateProperty); }
            set { SetValue(DetailsTemplateProperty, value); }
        }

        public object Details
        {
            get { return GetValue(DetailsProperty); }
            set { SetValue(DetailsProperty, value); }
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
            DetailsPresenter = GetTemplateChild(DetailsPresenterName) as ContentControl;

            // Visual States
            AdaptiveVisualStateGroupElement = GetTemplateChild(AdaptiveVisualStateGroupName) as VisualStateGroup;
            NormalVisualStateElement = GetTemplateChild(NormalVisualStateName) as VisualState;
            NarrowVisualStateElement = GetTemplateChild(NarrowVisualStateName) as VisualState;

            if (AdaptiveVisualStateGroupElement != null)
                AdaptiveVisualStateGroupElement.CurrentStateChanged += AdaptiveVisualStateGroupElement_CurrentStateChanged;

            // Animations
            DetailDrillIn = GetTemplateChild(DetailsDrillInAnimationName) as Storyboard;
            DetailDrillOut = GetTemplateChild(DetailsDrillOutAnimationName) as Storyboard;
        }

        private void UpdateViewState()
        {
            var previousState = _previousState ?? NormalVisualStateElement;
            if (previousState == NormalVisualStateElement &&
                AdaptiveVisualStateGroupElement.CurrentState == NarrowVisualStateElement)
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

                OnViewStateChanged();
            }
            else if (previousState == NarrowVisualStateElement &&
                AdaptiveVisualStateGroupElement.CurrentState == NormalVisualStateElement)
            {
                // master -> normal
                // details -> normal
                ResetViewForNormal();
                OnViewStateChanged();
            }
            else if (previousState == NarrowVisualStateElement &&
                AdaptiveVisualStateGroupElement.CurrentState == NarrowVisualStateElement)
            {
                if (IsDetailsViewInStackedMode)
                {// master -> details
                    DetailDrillIn.Begin();
                    SetHitTestVisibility(MasterPresenter, false);
                    SetHitTestVisibility(DetailsPresenter, true);
                }
                else
                {
                    // details -> master
                    DetailDrillOut.Begin();
                    SetHitTestVisibility(MasterPresenter, true);
                    SetHitTestVisibility(DetailsPresenter, false);
                }

                OnViewStateChanged();
            }

            UpdateReadonlyStateProperties();
        }

        private void UpdateReadonlyStateProperties()
        {
            if (AdaptiveVisualStateGroupElement == null)
                return;

            IsStackedMode = AdaptiveVisualStateGroupElement.CurrentState == NarrowVisualStateElement;

            CanExitDetailsView = IsDetailsViewInStackedMode &&
                AdaptiveVisualStateGroupElement.CurrentState == NarrowVisualStateElement;
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
