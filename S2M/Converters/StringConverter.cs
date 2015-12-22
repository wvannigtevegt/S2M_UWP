using System;
using Windows.UI.Xaml.Data;

namespace S2M.Converters {
	/// <summary>
	/// The string format converter.
	/// </summary>
	public class StringConverter : IValueConverter {
		/// <summary>
		/// Converts a value to a formatted string using the parameter value as the returned string value formatted with the given value.
		/// </summary>
		/// <example>
		/// <TextBlock Text="{Binding Collection.Count, Converter={StaticResource StringConverter}, ConverterParameter='Number of people: {0}'}" />
		/// </example>
		/// <returns>
		/// Returns a formatted string.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, string language) {
			var val = parameter as string;
			return string.IsNullOrWhiteSpace(val) ? value.ToString() : string.Format(val, value);
		}

		/// <summary>
		/// ConvertBack is not supported for the StringFormatConverter.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotImplementedException();
		}
	}
}
