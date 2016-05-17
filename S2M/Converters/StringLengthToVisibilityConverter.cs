using S2M.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace S2M.Converters
{
	public class StringLengthToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var invert = (parameter != null) && System.Convert.ToBoolean(parameter);

			var isVisible = false;
			if (value != null)
			{
				string stringValue = string.IsNullOrEmpty((string)value) ? "" : (string)value;
				isVisible = stringValue.Length > 0;
			}

			if (invert)
			{
				isVisible = !isVisible;
			}

			return isVisible.ToVisibility();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
