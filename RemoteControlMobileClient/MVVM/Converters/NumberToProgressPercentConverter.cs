using System.Globalization;

namespace RemoteControlMobileClient.MVVM.Converters
{
	public class NumberToProgressPercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return 0;
			if (parameter == null) return value;
			
			_ = double.TryParse(value.ToString(), out double progress);
			_ = double.TryParse(parameter.ToString(), out double max);
			return progress / max;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
