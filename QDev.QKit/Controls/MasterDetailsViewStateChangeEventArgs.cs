using Windows.UI.Xaml;

namespace QDev.QKit.Controls
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
