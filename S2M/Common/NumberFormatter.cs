using System;
using Windows.UI.Xaml.Data;

namespace S2M.Common {
	public class NumberFormatter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			try {
				return System.Convert.ToInt32(value).ToString(parameter as string);
			}
			catch (Exception ex) {
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}

	}
}
