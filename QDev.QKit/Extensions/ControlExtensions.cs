using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QDev.QKit.Extensions
{
    public class ControlExtensions
    {
        public static readonly DependencyProperty FocusWhenMadeVisibleProperty =
            DependencyProperty.RegisterAttached("FocusWhenMadeVisible",
                typeof(bool),
                typeof(ControlExtensions),
                new PropertyMetadata(false, IsFocusedPropertyChanged));

        private static readonly DependencyProperty FocusWhenMadeVisibleCallbackTokenProperty =
            DependencyProperty.RegisterAttached("FocusWhenMadeVisibleCallbackToken",
                typeof(long?),
                typeof(ControlExtensions),
                new PropertyMetadata(null));

        public static bool GetFocusWhenMadeVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusWhenMadeVisibleProperty);
        }

        public static void SetFocusWhenMadeVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusWhenMadeVisibleProperty, value);
        }

        private static long? GetFocusWhenMadeVisibleCallbackToken(DependencyObject obj)
        {
            return (long?)obj.GetValue(FocusWhenMadeVisibleCallbackTokenProperty);
        }

        private static void SetFocusWhenMadeVisibleCallbackToken(DependencyObject obj, long? value)
        {
            obj.SetValue(FocusWhenMadeVisibleCallbackTokenProperty, value);
        }

        private static void IsFocusedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = obj as Control;

            if (control == null)
                return;

            var token = GetFocusWhenMadeVisibleCallbackToken(control);

            if (token.HasValue)
            {
                control.UnregisterPropertyChangedCallback(UIElement.VisibilityProperty, token.Value);
                SetFocusWhenMadeVisibleCallbackToken(control, null);
            }

            if ((bool)e.NewValue)
            {
                var newToken = control.RegisterPropertyChangedCallback(UIElement.VisibilityProperty, VisibilityChangedCallback);
                SetFocusWhenMadeVisibleCallbackToken(control, newToken);
            }
        }

        private static void VisibilityChangedCallback(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is Control control && control.Visibility == Visibility.Visible)
            {
                var isSuccessful = control.Focus(FocusState.Programmatic);

                if (!isSuccessful)
                    control.SizeChanged += Control_SizeChanged;
            }
        }

        private static void Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var control = sender as Control;

            if (control == null)
                return;

            var isSuccessful = control.Focus(FocusState.Programmatic);

            if (isSuccessful)
                control.SizeChanged -= Control_SizeChanged;
        }
    }
}
