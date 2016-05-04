using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace S2M.Converters
{
    /// <summary>
    /// This converter helps mapping the selection state of a list legend item
    /// to a color value.
    /// </summary>
    public class SelectedLegendItemToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isSelected = (bool) value;

            return isSelected
                ? Application.Current.Resources["SystemControlForegroundBaseMediumBrush"] as Brush
                : Application.Current.Resources["SystemControlForegroundListMediumBrush"] as Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}