using System;
using Windows.UI.Xaml.Data;

namespace S2M.Common {
	public class StringFormatter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			string formatString = parameter as string;
			if (!string.IsNullOrEmpty(formatString)) {
				return string.Format(formatString, value);
			}

			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
