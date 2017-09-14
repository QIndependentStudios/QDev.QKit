using Windows.UI.Xaml;

namespace QKit.Uwp.Controls
{
    public class MasterDetailsViewStateChangeEventArgs : RoutedEventArgs
    {
        public MasterDetailsViewStateChangeEventArgs(MasterDetailsViewState oldValue, MasterDetailsViewState newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public MasterDetailsViewState OldValue { get; private set; }
        public MasterDetailsViewState NewValue { get; private set; }
    }
}
