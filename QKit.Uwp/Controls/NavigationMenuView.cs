using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace QKit.Uwp.Controls
{
    [TemplatePart(Name = RootSplitViewName, Type = typeof(SplitView))]
    [TemplateVisualState(Name = WideVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [TemplateVisualState(Name = NormalVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [TemplateVisualState(Name = NarrowVisualStateName, GroupName = AdaptiveVisualStateGroupName)]
    [ContentProperty(Name = nameof(PrimaryMenuItems))]
    public sealed class NavigationMenuView : Control
    {
        #region Instances
        internal static List<NavigationMenuView> Instances = new List<NavigationMenuView>();
        #endregion

        #region Events
        public event RoutedEventHandler SelectedMenuItemChanged;
        #endregion

        #region Constants
        private const string RootSplitViewName = "RootSplitView";
        private const string NavigationMenuItemsGroupName = "NavigationMenuView.NavigationMenuItems";
        public const string AdaptiveVisualStateGroupName = "AdaptiveVisualStateGroup";
        public const string WideVisualStateName = "WideVisualState";
        public const string NormalVisualStateName = "NormalVisualState";
        public const string NarrowVisualStateName = "NarrowVisualState";
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty PrimaryMenuItemsProperty = DependencyProperty.Register(
            nameof(PrimaryMenuItems),
            typeof(ObservableCollection<NavigationMenuItem>),
            typeof(NavigationMenuView),
            new PropertyMetadata(new ObservableCollection<NavigationMenuItem>(), OnMenuItemsCollectionChanged));

        public static readonly DependencyProperty SecondaryMenuItemsProperty = DependencyProperty.Register(
            nameof(SecondaryMenuItems),
            typeof(ObservableCollection<NavigationMenuItem>),
            typeof(NavigationMenuView),
            new PropertyMetadata(new ObservableCollection<NavigationMenuItem>(), OnMenuItemsCollectionChanged));

        public static readonly DependencyProperty MainMenuButtonIconProperty = DependencyProperty.Register(
            nameof(MainMenuButtonIcon),
            typeof(IconElement),
            typeof(NavigationMenuView),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MainMenuButtonContentProperty = DependencyProperty.Register(
            nameof(MainMenuButtonContent),
            typeof(object),
            typeof(NavigationMenuView),
            new PropertyMetadata(null));

        public static readonly DependencyProperty NavigationFrameProperty = DependencyProperty.Register(
            nameof(NavigationFrame),
            typeof(Frame),
            typeof(NavigationMenuView),
            new PropertyMetadata(null,
                (sender, args) =>
                {
                    if (sender is NavigationMenuView control)
                        control.SetSplitViewContent();
                }));

        public static readonly DependencyProperty SelectedMenuItemProperty = DependencyProperty.Register(
            nameof(SelectedMenuItem),
            typeof(NavigationMenuItem),
            typeof(NavigationMenuView),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FontIconGlyphSizeProperty = DependencyProperty.Register(
            nameof(FontIconGlyphSize),
            typeof(double),
            typeof(NavigationMenuView),
            new PropertyMetadata(default(double),
                (sender, args) =>
                {
                    if (sender is NavigationMenuView control)
                    {
                        foreach (var menuItem in control.PrimaryMenuItems)
                        {
                            control.SetGlyphFontSize(menuItem);
                        }
                    }
                }));

        public static readonly DependencyProperty IsMenuOpenProperty = DependencyProperty.Register(
            nameof(IsMenuOpen),
            typeof(bool),
            typeof(NavigationMenuView),
            new PropertyMetadata(default(bool),
                (sender, args) =>
                {
                    if (sender is NavigationMenuView control)
                        control.SetSplitViewPaneIsOpen((bool)args.NewValue);
                }));

        public static readonly DependencyProperty NormalStateMinWidthProperty = DependencyProperty.Register(
            nameof(NormalStateMinWidth),
            typeof(double),
            typeof(NavigationMenuView),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty WideStateMinWidthProperty = DependencyProperty.Register(
            nameof(WideStateMinWidth),
            typeof(double),
            typeof(NavigationMenuView),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty CompactMenuWidthProperty = DependencyProperty.Register(
            nameof(CompactMenuWidth),
            typeof(double),
            typeof(NavigationMenuView),
            new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MenuPaneWidthProperty = DependencyProperty.Register(
            nameof(MenuPaneWidth),
            typeof(double),
            typeof(NavigationMenuView),
            new PropertyMetadata(default(double)));
        #endregion

        #region Template Parts
        private SplitView RootSplitView { get; set; }

        private VisualStateGroup AdaptiveVisualStateGroup { get; set; }
        private VisualState WideVisualState { get; set; }
        private VisualState NormalVisualState { get; set; }
        private VisualState NarrowVisualState { get; set; }
        #endregion

        #region Constructors
        public NavigationMenuView()
        {
            DefaultStyleKey = typeof(NavigationMenuView);
            Instances.Add(this);
            Unloaded += NavigationMenuView_Unloaded;
        }
        #endregion

        #region Properties
        public ObservableCollection<NavigationMenuItem> PrimaryMenuItems
        {
            get { return (ObservableCollection<NavigationMenuItem>)GetValue(PrimaryMenuItemsProperty); }
            set { SetValue(PrimaryMenuItemsProperty, value); }
        }

        public ObservableCollection<NavigationMenuItem> SecondaryMenuItems
        {
            get { return (ObservableCollection<NavigationMenuItem>)GetValue(SecondaryMenuItemsProperty); }
            set { SetValue(SecondaryMenuItemsProperty, value); }
        }

        public IconElement MainMenuButtonIcon
        {
            get { return (IconElement)GetValue(MainMenuButtonIconProperty); }
            set { SetValue(MainMenuButtonIconProperty, value); }
        }

        public object MainMenuButtonContent
        {
            get { return GetValue(MainMenuButtonContentProperty); }
            set { SetValue(MainMenuButtonContentProperty, value); }
        }

        public Frame NavigationFrame
        {
            get { return (Frame)GetValue(NavigationFrameProperty); }
            set { SetValue(NavigationFrameProperty, value); }
        }

        public NavigationMenuItem SelectedMenuItem
        {
            get { return (NavigationMenuItem)GetValue(SelectedMenuItemProperty); }
            private set { SetValue(SelectedMenuItemProperty, value); }
        }

        public double FontIconGlyphSize
        {
            get { return (double)GetValue(FontIconGlyphSizeProperty); }
            set { SetValue(FontIconGlyphSizeProperty, value); }
        }

        public bool IsMenuOpen
        {
            get { return (bool)GetValue(IsMenuOpenProperty); }
            set { SetValue(IsMenuOpenProperty, value); }
        }

        public double NormalStateMinWidth
        {
            get { return (double)GetValue(NormalStateMinWidthProperty); }
            set { SetValue(NormalStateMinWidthProperty, value); }
        }

        public double WideStateMinWidth
        {
            get { return (double)GetValue(WideStateMinWidthProperty); }
            set { SetValue(WideStateMinWidthProperty, value); }
        }

        public double CompactMenuWidth
        {
            get { return (double)GetValue(CompactMenuWidthProperty); }
            set { SetValue(CompactMenuWidthProperty, value); }
        }

        public double MenuPaneWidth
        {
            get { return (double)GetValue(MenuPaneWidthProperty); }
            set { SetValue(MenuPaneWidthProperty, value); }
        }
        #endregion

        #region Methods
        protected override void OnApplyTemplate()
        {
            RootSplitView = GetTemplateChild(RootSplitViewName) as SplitView;
            RootSplitView.PaneClosed += RootSplitView_PaneClosed;

            AdaptiveVisualStateGroup = GetTemplateChild(AdaptiveVisualStateGroupName) as VisualStateGroup;
            WideVisualState = GetTemplateChild(WideVisualStateName) as VisualState;
            NormalVisualState = GetTemplateChild(NormalVisualStateName) as VisualState;
            NarrowVisualState = GetTemplateChild(NarrowVisualStateName) as VisualState;

            if (AdaptiveVisualStateGroup != null)
                AdaptiveVisualStateGroup.CurrentStateChanged += AdaptiveVisualStateGroupElement_CurrentStateChanged;

            UpdateSplitViewStates();
            SetSplitViewContent();
            RegisterNewMenuItemCollection(PrimaryMenuItems);
            RegisterNewMenuItemCollection(SecondaryMenuItems);
        }

        private void UpdateSplitViewStates()
        {
            IsMenuOpen = false;
            if (RootSplitView != null)
            {
                if (AdaptiveVisualStateGroup.CurrentState == WideVisualState)
                {
                    RootSplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
                    IsMenuOpen = true;
                }
                else if (AdaptiveVisualStateGroup.CurrentState == NormalVisualState)
                    RootSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                else
                    RootSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
            }
        }

        private void SetSplitViewContent()
        {
            if (RootSplitView == null)
                return;

            RootSplitView.Content = NavigationFrame;
        }

        private void SetGlyphFontSize(NavigationMenuItem menuItem)
        {
            if (menuItem.Icon is FontIcon fontIcon)
                fontIcon.FontSize = FontIconGlyphSize;
        }

        private void RegisterNewMenuItemCollection(ObservableCollection<NavigationMenuItem> menuItems)
        {
            if (menuItems == null)
                return;

            foreach (var menuItem in menuItems)
            {
                RegisterMenuItem(menuItem);
            }

            menuItems.CollectionChanged -= MenuItems_CollectionChanged;
            menuItems.CollectionChanged += MenuItems_CollectionChanged;
        }

        private void RegisterMenuItem(NavigationMenuItem menuItem)
        {
            SetGlyphFontSize(menuItem);

            menuItem.GroupName = NavigationMenuItemsGroupName;
            menuItem.Checked -= MenuItem_Checked;
            menuItem.Checked += MenuItem_Checked;
        }

        private void UnregisterMenuItem(NavigationMenuItem menuItem)
        {
            menuItem.GroupName = null;
            menuItem.Checked -= MenuItem_Checked;
        }

        private void SetSplitViewPaneIsOpen(bool isOpen)
        {
            if (RootSplitView != null && RootSplitView.IsPaneOpen != isOpen)
                RootSplitView.IsPaneOpen = isOpen;
        }

        private void OnSelectedMenuItemChanged()
        {
            if (SelectedMenuItemChanged == null)
                return;

            var args = new RoutedEventArgs();
            SelectedMenuItemChanged(this, args);
        }
        #endregion

        #region Event Handlers
        private static void OnMenuItemsCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NavigationMenuView;
            if (control == null)
                return;

            if (e.OldValue is ObservableCollection<NavigationMenuItem> oldCollection)
            {
                oldCollection.CollectionChanged -= control.MenuItems_CollectionChanged;

                foreach (var menuItem in oldCollection)
                {
                    control.UnregisterMenuItem(menuItem);
                }
            }

            var newCollection = e.NewValue as ObservableCollection<NavigationMenuItem>;
            control.RegisterNewMenuItemCollection(newCollection);
        }

        private void NavigationMenuView_Unloaded(object sender, RoutedEventArgs e)
        {
            Instances.Remove(this);
        }

        private void RootSplitView_PaneClosed(SplitView sender, object args)
        {
            IsMenuOpen = false;
        }

        private void AdaptiveVisualStateGroupElement_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateSplitViewStates();
        }

        private void MenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var menuItem in e.OldItems)
                {
                    UnregisterMenuItem((NavigationMenuItem)menuItem);
                }

            if (e.NewItems != null)
                foreach (var menuItem in e.NewItems)
                {
                    RegisterMenuItem((NavigationMenuItem)menuItem);
                }
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            SelectedMenuItem = sender as NavigationMenuItem;

            if (AdaptiveVisualStateGroup != null &&
                AdaptiveVisualStateGroup.CurrentState != WideVisualState)
                IsMenuOpen = false;

            OnSelectedMenuItemChanged();
        }
        #endregion
    }
}
