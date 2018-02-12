using QDev.QKit.Controls;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace QDev.QKit.Converters
{
    public class MasterDetailsSelectionModeConverter : IValueConverter
    {
        public ListViewSelectionMode FullSelectionMode { get; set; } = ListViewSelectionMode.Single;
        public ListViewSelectionMode MasterSelectionMode { get; set; } = ListViewSelectionMode.None;
        public ListViewSelectionMode DetailsSelectionMode { get; set; } = ListViewSelectionMode.None;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MasterDetailsViewState state)
                switch (state)
                {
                    case MasterDetailsViewState.Full:
                        return FullSelectionMode;
                    case MasterDetailsViewState.Master:
                        return MasterSelectionMode;
                    case MasterDetailsViewState.Details:
                        return DetailsSelectionMode;
                    default:
                        break;
                }

            return ListViewSelectionMode.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
