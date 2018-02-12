using Windows.UI.Xaml;

namespace QDev.QKit.Triggers
{
    public class ControlSizeTrigger : StateTriggerBase
    {
        #region DependencyProperties
        public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register(
            nameof(TargetElement),
            typeof(FrameworkElement),
            typeof(ControlSizeTrigger),
            new PropertyMetadata(default(FrameworkElement),
                (sender, args) =>
                {
                    var trigger = sender as ControlSizeTrigger;
                    var targetElement = args.NewValue as FrameworkElement;

                    if (trigger == null || targetElement == null)
                        return;

                    if (targetElement != null)
                        targetElement.SizeChanged -= trigger.TargetElement_SizeChanged;

                    if (targetElement != null)
                        targetElement.SizeChanged += trigger.TargetElement_SizeChanged;
                }));

        public static readonly DependencyProperty MinimumHeightProperty = DependencyProperty.Register(
            nameof(MinimumHeight),
            typeof(double),
            typeof(ControlSizeTrigger),
            new PropertyMetadata(double.NaN, OnMinimumSizeChanged));

        public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.Register(
            nameof(MinimumWidth),
            typeof(double),
            typeof(ControlSizeTrigger),
            new PropertyMetadata(double.NaN, OnMinimumSizeChanged));
        #endregion

        #region Properties
        public FrameworkElement TargetElement
        {
            get { return (FrameworkElement)GetValue(TargetElementProperty); }
            set { SetValue(TargetElementProperty, value); }
        }

        public double MinimumHeight
        {
            get { return (double)GetValue(MinimumHeightProperty); }
            set { SetValue(MinimumHeightProperty, value); }
        }

        public double MinimumWidth
        {
            get { return (double)GetValue(MinimumWidthProperty); }
            set { SetValue(MinimumWidthProperty, value); }
        }
        #endregion

        #region Methods
        protected double GetCurrentHeight()
        {
            return TargetElement == null
                ? double.NaN
                : TargetElement.ActualHeight;
        }

        protected double GetCurrentWidth()
        {
            return TargetElement == null
                ? double.NaN
                : TargetElement.ActualWidth;
        }

        protected void UpdateIsActive()
        {
            SetActive((double.IsNaN(MinimumWidth) || GetCurrentWidth() >= MinimumWidth) &&
                (double.IsNaN(MinimumHeight) || GetCurrentHeight() >= MinimumHeight));
        }
        #endregion

        #region Event Handlers
        private void TargetElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateIsActive();
        }

        private static void OnMinimumSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as ControlSizeTrigger;

            if (trigger == null)
                return;

            trigger.UpdateIsActive();
        }
        #endregion
    }

}
