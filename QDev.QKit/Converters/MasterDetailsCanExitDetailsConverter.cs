using QDev.QKit.Controls;
using System;
using Windows.UI.Xaml.Data;

namespace QDev.QKit.Converters
{
    public class MasterDetailsCanExitDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MasterDetailsViewState state)
                return state == MasterDetailsViewState.Details;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
