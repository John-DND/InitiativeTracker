using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using InitiativeTracker.Data.Tray;

namespace InitiativeTracker.Data.Util
{
    public class IsNullOrEmptyConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            if (value == null) return true;
            if (value is ICollection collection) return collection.Count == 0;
            if (value is string str) return str.Length == 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
