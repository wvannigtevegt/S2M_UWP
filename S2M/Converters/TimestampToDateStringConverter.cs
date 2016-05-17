using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace S2M.Converters
{
	public class TimestampToDateStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var timestamp = (long)value;
			var dateFormat = (string)parameter;
			var outputFormat = "dd MMM yyyy";
			if (!string.IsNullOrEmpty(dateFormat)) {
				outputFormat = dateFormat;
			}

			return Common.DateService.ConvertFromUnixTimestamp(timestamp).ToString(outputFormat);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
