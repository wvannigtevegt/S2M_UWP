using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace S2M.Converters
{
	public class CheckInToIsPresentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var checkin = (Models.CheckIn)value;
			if (checkin != null)
			{
				if (checkin.IsConfirmed && !checkin.HasLeft)
				{
					return new SolidColorBrush(Colors.Green);
				}

				if (checkin.HasLeft)
				{
					return new SolidColorBrush(Colors.Gray);
				}
			}

			return new SolidColorBrush(Colors.DarkRed);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
