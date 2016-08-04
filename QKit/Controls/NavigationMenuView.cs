using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QKit.Controls
{
    [TemplatePart(Name = RootSplitViewName, Type = typeof(SplitView))]
    public sealed class NavigationMenuView : Control
    {
        public delegate void SelectedMenuItemChangedEventHandler(object sender, RoutedEventArgs e);
        public event SelectedMenuItemChangedEventHandler SelectedMenuItemChanged;

        #region DependencyProperties
        public static readonly DependencyProperty PrimaryMenuItemsProperty = DependencyProperty.Register(
            nameof(PrimaryMenuItems),
            typeof(ObservableCollection<NavigationMenuItem>),
            typeof(NavigationMenuView),
            new PropertyMetadata(new ObservableCollection<NavigationMenuItem>(),
                (sender, args) =>
                {
                    var control = sender as NavigationMenuView;
                    if (control == null)
                        return;

                    var oldCollection = args.OldValue as ObservableCollection<NavigationMenuItem>;
                    if (oldCollection != null)
                    {
                        oldCollection.CollectionChanged -= control.PrimaryMenuItems_CollectionChanged;

                        foreach (var menuItem in oldCollection)
                        {
                            control.UnregisterMenuItem(menuItem);
                        }
                    }

                    var newCollection = args.NewValue as ObservableCollection<NavigationMenuItem>;
                    control.RegisterNewMenuItemCollection(newCollection);
                }));

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
                    var control = sender as NavigationMenuView;

                    if (control == null)
                        return;

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
                    var control = sender as NavigationMenuView;

                    if (control == null)
                        return;

                    foreach (var menuItem in control.PrimaryMenuItems)
                    {
                        control.SetGlyphFontSize(menuItem);
                    }
                }));
        #endregion

        #region Constants
        private const string RootSplitViewName = "RootSplitView";
        private const string NavigationMenuItemsGroupName = "NavigationMenuView.NavigationMenuItems";
        #endregion

        #region Constructors
        public NavigationMenuView()
        {
            DefaultStyleKey = typeof(NavigationMenuView);
        }
        #endregion

        #region Properties
        private SplitView RootSplitView { get; set; }

        public ObservableCollection<NavigationMenuItem> PrimaryMenuItems
        {
            get { return (ObservableCollection<NavigationMenuItem>)GetValue(PrimaryMenuItemsProperty); }
            set { SetValue(PrimaryMenuItemsProperty, value); }
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
            private set { SetValue(FontIconGlyphSizeProperty, value); }
        }
        #endregion

        #region Methods
        protected override void OnApplyTemplate()
        {
            RootSplitView = (SplitView)GetTemplateChild(RootSplitViewName);
            SetSplitViewContent();
            RegisterNewMenuItemCollection(PrimaryMenuItems);
        }

        private void SetSplitViewContent()
        {
            if (RootSplitView == null)
                return;

            RootSplitView.Content = NavigationFrame;
        }

        private void SetGlyphFontSize(NavigationMenuItem menuItem)
        {
            var fontIcon = menuItem.Icon as FontIcon;
            if (fontIcon != null)
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

            PrimaryMenuItems.CollectionChanged -= PrimaryMenuItems_CollectionChanged;
            PrimaryMenuItems.CollectionChanged += PrimaryMenuItems_CollectionChanged;
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

        private void OnSelectedMenuItemChanged()
        {
            if (SelectedMenuItemChanged == null)
                return;

            var args = new RoutedEventArgs();
            SelectedMenuItemChanged(this, args);
        }
        #endregion

        #region Event Handlers
        private void PrimaryMenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            OnSelectedMenuItemChanged();
        }
        #endregion
    }
}
