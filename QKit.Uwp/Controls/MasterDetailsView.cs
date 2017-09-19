using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace QKit.Uwp.Controls
{
    [TemplatePart(Name = CommonStatesName, Type = typeof(VisualStateGroup))]
    [TemplatePart(Name = FullVisualStateName, Type = typeof(VisualState))]
    [TemplatePart(Name = MasterVisualStateName, Type = typeof(VisualState))]
    [TemplatePart(Name = DetailsVisualStateName, Type = typeof(VisualState))]
    [TemplatePart(Name = MasterPresenterName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = DetailsPresenterName, Type = typeof(ContentPresenter))]
    [ContentProperty(Name = nameof(DetailsContent))]
    public sealed class MasterDetailsView : Control
    {
        #region Events
        public event EventHandler<MasterDetailsViewStateChangeEventArgs> ViewStateChanging;
        public event EventHandler<MasterDetailsViewStateChangeEventArgs> ViewStateChanged;
        #endregion

        #region Constants
        private const string CommonStatesName = "CommonStates";
        private const string FullVisualStateName = "FullVisualState";
        private const string MasterVisualStateName = "MasterVisualState";
        private const string DetailsVisualStateName = "DetailsVisualState";
        private const string MasterPresenterName = "MasterPresenter";
        private const string DetailsPresenterName = "DetailsPresenter";

        private static Dictionary<string, MasterDetailsViewState> VisualStateNameEnumDictionary = new Dictionary<string, MasterDetailsViewState>
        {
            { FullVisualStateName, MasterDetailsViewState.Full },
            { MasterVisualStateName, MasterDetailsViewState.Master },
            { DetailsVisualStateName, MasterDetailsViewState.Details },
        };
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty StackedWidthThresholdProperty = DependencyProperty.Register(
            nameof(StackedWidthThreshold),
            typeof(double),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(double),
                (sender, args) =>
                {
                    if (sender is MasterDetailsView control)
                        control.UpdateVisualState();
                }));

        public static readonly DependencyProperty MasterPaneWidthProperty = DependencyProperty.Register(
            nameof(MasterPaneWidth),
            typeof(double),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MasterContentProperty = DependencyProperty.Register(
            nameof(MasterContent),
            typeof(object),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty DetailsContentProperty = DependencyProperty.Register(
            nameof(DetailsContent),
            typeof(object),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty MasterContentTemplateProperty = DependencyProperty.Register(
            nameof(MasterContentTemplate),
            typeof(DataTemplate),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty DetailsContentTemplateProperty = DependencyProperty.Register(
            nameof(DetailsContentTemplate),
            typeof(DataTemplate),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty MasterContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(MasterContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty DetailsContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(DetailsContentTemplateSelector),
            typeof(DataTemplate),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(
            nameof(ViewState),
            typeof(MasterDetailsViewState),
            typeof(MasterDetailsView),
            new PropertyMetadata(MasterDetailsViewState.Full));

        public static readonly DependencyProperty ShowDetailsInStackedModeProperty = DependencyProperty.Register(
            nameof(ShowDetailsInStackedMode),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(default(bool),
                (sender, args) =>
                {
                    if (sender is MasterDetailsView control)
                        control.UpdateVisualState();
                }));

        public static readonly DependencyProperty UseTransitionsProperty = DependencyProperty.Register(
            nameof(UseTransitions),
            typeof(bool),
            typeof(MasterDetailsView),
            new PropertyMetadata(true));
        #endregion

        #region Template Parts
        private VisualStateGroup CommonStates { get; set; }
        #endregion

        #region Properties
        public double StackedWidthThreshold
        {
            get { return (double)GetValue(StackedWidthThresholdProperty); }
            set { SetValue(StackedWidthThresholdProperty, value); }
        }

        public double MasterPaneWidth
        {
            get { return (double)GetValue(MasterPaneWidthProperty); }
            set { SetValue(MasterPaneWidthProperty, value); }
        }

        public object MasterContent
        {
            get { return GetValue(MasterContentProperty); }
            set { SetValue(MasterContentProperty, value); }
        }

        public object DetailsContent
        {
            get { return GetValue(DetailsContentProperty); }
            set { SetValue(DetailsContentProperty, value); }
        }

        public DataTemplate MasterContentTemplate
        {
            get { return (DataTemplate)GetValue(MasterContentTemplateProperty); }
            set { SetValue(MasterContentTemplateProperty, value); }
        }

        public DataTemplate DetailsContentTemplate
        {
            get { return (DataTemplate)GetValue(DetailsContentTemplateProperty); }
            set { SetValue(DetailsContentTemplateProperty, value); }
        }

        public DataTemplateSelector MasterContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MasterContentTemplateSelectorProperty); }
            set { SetValue(MasterContentTemplateSelectorProperty, value); }
        }

        public DataTemplateSelector DetailsContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DetailsContentTemplateSelectorProperty); }
            set { SetValue(DetailsContentTemplateSelectorProperty, value); }
        }

        public MasterDetailsViewState ViewState
        {
            get { return (MasterDetailsViewState)GetValue(ViewStateProperty); }
            private set { SetValue(ViewStateProperty, value); }
        }

        public bool ShowDetailsInStackedMode
        {
            get { return (bool)GetValue(ShowDetailsInStackedModeProperty); }
            set { SetValue(ShowDetailsInStackedModeProperty, value); }
        }

        public bool UseTransitions
        {
            get { return (bool)GetValue(UseTransitionsProperty); }
            set { SetValue(UseTransitionsProperty, value); }
        }
        #endregion

        #region Constructors
        public MasterDetailsView()
        {
            DefaultStyleKey = typeof(MasterDetailsView);
            Loaded += MasterDetailsView_Loaded;
            Unloaded += MasterDetailsView_Unloaded;
        }
        #endregion

        #region Methods
        private void UpdateVisualState()
        {
            var oldViewState = ViewState;
            var newViewState = MasterDetailsViewState.Full;
            var newViewStateName = FullVisualStateName;

            if (ActualWidth < StackedWidthThreshold)
            {
                newViewStateName = ShowDetailsInStackedMode ? DetailsVisualStateName : MasterVisualStateName;

                if (VisualStateNameEnumDictionary.ContainsKey(newViewStateName))
                    newViewState = VisualStateNameEnumDictionary[newViewStateName];
            }

            if (newViewState != MasterDetailsViewState.Details)
                GetTemplateChild(MasterPresenterName);

            if (newViewState != MasterDetailsViewState.Master)
                GetTemplateChild(DetailsPresenterName);

            ViewState = newViewState;
            VisualStateManager.GoToState(this,
                newViewStateName,
                UseTransitions && newViewState != MasterDetailsViewState.Full);

            OnViewStateChanging(oldViewState, newViewState);
        }

        private void OnViewStateChanging(MasterDetailsViewState oldValue, MasterDetailsViewState newValue)
        {
            if (ViewStateChanging == null || oldValue == newValue)
                return;

            var args = new MasterDetailsViewStateChangeEventArgs(oldValue, newValue);
            ViewStateChanging(this, args);
        }

        private void OnViewStateChanged(MasterDetailsViewState oldValue, MasterDetailsViewState newValue)
        {
            if (ViewStateChanged == null || oldValue == newValue)
                return;

            var args = new MasterDetailsViewStateChangeEventArgs(oldValue, newValue);
            ViewStateChanged(this, args);
        }

        protected override void OnApplyTemplate()
        {
            if (CommonStates != null)
                CommonStates.CurrentStateChanged -= CommonStates_CurrentStateChanged;

            CommonStates = GetTemplateChild(CommonStatesName) as VisualStateGroup;

            if (CommonStates != null)
                CommonStates.CurrentStateChanged += CommonStates_CurrentStateChanged;
        }
        #endregion

        #region Event Handlers
        private void MasterDetailsView_Loaded(object sender, RoutedEventArgs e)
        {
            SizeChanged += MasterDetailsView_SizeChanged;
            UpdateVisualState();
        }

        private void MasterDetailsView_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= MasterDetailsView_SizeChanged;
        }

        private void MasterDetailsView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVisualState();
        }

        private void CommonStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            var oldValue = MasterDetailsViewState.Full;
            if (e.OldState != null && VisualStateNameEnumDictionary.ContainsKey(e.OldState.Name))
                oldValue = VisualStateNameEnumDictionary[e.OldState.Name];

            var newValue = MasterDetailsViewState.Full;
            if (VisualStateNameEnumDictionary.ContainsKey(e.NewState.Name))
                newValue = VisualStateNameEnumDictionary[e.NewState.Name];

            OnViewStateChanged(oldValue, newValue);
        }
        #endregion
    }
}
