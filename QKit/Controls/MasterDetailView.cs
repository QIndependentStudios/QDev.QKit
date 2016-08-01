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
            new PropertyMetadata(default(object),
                (sender, args) =>
                {
                    var control = sender as MasterDetailsView;

                    if (control == null || control.AdaptiveVisualStateGroupElement == null)
                        return;

                    control.IsDetailsViewPreferred = args.NewValue != null;

                    if (control.AdaptiveVisualStateGroupElement.CurrentState != control.NormalVisualStateElement)
                    {
                        if (control.IsDetailsViewPreferred && control.DetailDrillIn != null)
                            control.DetailDrillIn.Begin();
                        else if (!control.IsDetailsViewPreferred && control.DetailDrillOut != null)
                            control.DetailDrillOut.Begin();
                    }
                    else
                    {
                        control.DetailDrillIn?.Stop();
                        control.DetailDrillOut?.Stop();
                    }
                }));

        public static readonly DependencyProperty IsDetailsViewPreferredProperty = DependencyProperty.Register(
            nameof(IsDetailsViewPreferred),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(bool),
                (sender, args) =>
                {
                    var control = sender as MasterDetailsView;

                    if (control == null)
                        return;

                    control.UpdateCanExitDetailsView();
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

        public bool IsDetailsViewPreferred
        {
            get { return (bool)GetValue(IsDetailsViewPreferredProperty); }
            private set { SetValue(IsDetailsViewPreferredProperty, value); }

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

        private void UpdateCanExitDetailsView()
        {
            if (AdaptiveVisualStateGroupElement == null)
                return;

            CanExitDetailsView = IsDetailsViewPreferred && 
                AdaptiveVisualStateGroupElement.CurrentState == NarrowVisualStateElement;
        }

        private void OnViewStateChanged()
        {
            if (ViewStateChanged == null)
                return;

            var args = new RoutedEventArgs();
            ViewStateChanged(this, args);
        }

        public void ExitDetailsView()
        {
            if (CanExitDetailsView)
            {
                IsDetailsViewPreferred = false;
                DetailDrillOut?.Begin();
                Details = null;
            }
        }
        #endregion

        #region Event Handlers
        private void AdaptiveVisualStateGroupElement_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {

            if (e.OldState == NormalVisualStateElement && e.NewState != NormalVisualStateElement)
            {
                if (IsDetailsViewPreferred && DetailDrillIn != null)
                {
                    DetailDrillIn.Begin();
                    DetailDrillIn.SkipToFill();
                }
                else if (!IsDetailsViewPreferred && DetailDrillOut != null)
                {
                    DetailDrillOut.Begin();
                    DetailDrillOut.SkipToFill();
                }
            }
            else if (e.NewState == NormalVisualStateElement)
            {
                DetailDrillIn?.Stop();
                DetailDrillOut?.Stop();
            }

            UpdateCanExitDetailsView();
            OnViewStateChanged();
        }
        #endregion
    }
}
