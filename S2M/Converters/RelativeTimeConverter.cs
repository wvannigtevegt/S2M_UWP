using S2M.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace S2M.Converters
{
	/// <summary>
	/// Does conversion between <see cref="DateTime" /> and a human
	/// readable string which specifies the time passed.
	/// </summary>
	public class RelativeTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var dateTime = (DateTime)value;
			return DateService.ToRelativeTime(dateTime);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
