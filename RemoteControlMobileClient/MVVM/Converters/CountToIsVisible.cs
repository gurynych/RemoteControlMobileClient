using RemoteControlMobileClient.BusinessLogic.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlMobileClient.MVVM.Converters
{
    internal class CountToIsVisible : IMarkupExtension, IValueConverter
    {
        static CountToIsVisible Instance = new CountToIsVisible();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                if (parameter is int  count2)
                {
                    return count > 0 || count2 > 0;
                }

                return count > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
