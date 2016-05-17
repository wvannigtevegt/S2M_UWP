using S2M.Extensions;
using System;
using Windows.UI.Xaml.Data;

namespace S2M.Converters
{
	public class AmountToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var invert = (parameter != null) && System.Convert.ToBoolean(parameter);

			var isVisible = (int)value > 0;

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
