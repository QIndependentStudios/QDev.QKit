using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace QKit.Controls
{
    [TemplateVisualState(Name = NormalVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [TemplateVisualState(Name = NarrowVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [ContentProperty(Name = nameof(Content))]
    public sealed class MasterDetailsView : ContentControl
    {
        public delegate void ViewStateChangedEventHandler(object sender, RoutedEventArgs e);
        public event ViewStateChangedEventHandler ViewStateChanged;

        #region Constants
        public const string AdaptiveVisualStateGroupName = "AdaptiveVisualStateGroup";
        public const string NormalVisualStateName = "VisualStateNormal";
        public const string NarrowVisualStateName = "VisualStateNarrow";
        public const string DetailsDrillInAnimationName = "DetailsDrillIn";
        public const string DetailsDrillOutAnimationName = "DetailsDrillOut";
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty VisualStateNormalMinWidthProperty = DependencyProperty.Register(
            nameof(VisualStateNormalMinWidth),
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

        public static readonly DependencyProperty IsDetailsViewStateProperty = DependencyProperty.Register(
            nameof(IsDetailsViewState),
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

        #region Visual States
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
        public double VisualStateNormalMinWidth
        {
            get { return (double)GetValue(VisualStateNormalMinWidthProperty); }
            set { SetValue(VisualStateNormalMinWidthProperty, value); }
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

        public bool IsDetailsViewState
        {
            get { return (bool)GetValue(IsDetailsViewStateProperty); }
            set { SetValue(IsDetailsViewStateProperty, value); }
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
            // Visual States
            AdaptiveVisualStateGroupElement = GetTemplateChild(AdaptiveVisualStateGroupName) as VisualStateGroup;
            NormalVisualStateElement = GetTemplateChild(NormalVisualStateName) as VisualState;
            NarrowVisualStateElement = GetTemplateChild(NarrowVisualStateName) as VisualState;

            if (AdaptiveVisualStateGroupElement != null)
                AdaptiveVisualStateGroupElement.CurrentStateChanged += AdaptiveVisualStateGroupElement_CurrentStateChanged;

            DetailDrillIn = GetTemplateChild(DetailsDrillInAnimationName) as Storyboard;
            DetailDrillOut = GetTemplateChild(DetailsDrillOutAnimationName) as Storyboard;
        }

        private void UpdateViewState()
        {
            var previousState = _previousState ?? NormalVisualStateElement;
            if (previousState == NormalVisualStateElement &&
                AdaptiveVisualStateGroupElement.CurrentState == NarrowVisualStateElement)
            {
                if (IsDetailsViewState)
                {
                    // normal -> master
                    DetailDrillIn.Begin();
                    DetailDrillIn.SkipToFill();
                }
                else
                {
                    // normal -> details
                    DetailDrillOut.Begin();
                    DetailDrillOut.SkipToFill();
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
                if (IsDetailsViewState)
                    // master -> details
                    DetailDrillIn.Begin();
                else
                    // details -> master
                    DetailDrillOut.Begin();

                OnViewStateChanged();
            }

            CanExitDetailsView = IsDetailsViewState &&
                AdaptiveVisualStateGroupElement.CurrentState == NarrowVisualStateElement;
        }

        private void UpdateCanExitDetailsView()
        {
            if (AdaptiveVisualStateGroupElement == null)
                return;

            CanExitDetailsView = IsDetailsViewState &&
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
