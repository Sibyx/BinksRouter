using System;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Data;

namespace BinksRouter.UI.Converters
{
    public class CutePhysicalAddress : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (PhysicalAddress) value;
            return item != null ? string.Join(":", item.GetAddressBytes().Select(b => b.ToString("X2"))) : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
