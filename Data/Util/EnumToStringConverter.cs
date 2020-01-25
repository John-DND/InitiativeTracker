using System;
using System.Globalization;
using System.Windows.Data;
using InitiativeTracker.Data.Tray;

namespace InitiativeTracker.Data.Util
{
    /// <summary>
    /// we can literally ignore half of the functionality provided by this interface, because we're only using it
    /// to convert an enum to a string
    /// </summary>
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Enum) value).ToString("G");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
