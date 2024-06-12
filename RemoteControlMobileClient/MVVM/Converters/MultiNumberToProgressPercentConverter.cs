using System.Globalization;

namespace RemoteControlMobileClient.MVVM.Converters
{
	public class MultiNumberToProgressPercentConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length < 2) return null;			

			_ = double.TryParse(values[0]?.ToString(), out double progress);
			_ = double.TryParse(values[1]?.ToString(), out double max);
			return progress / max;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
