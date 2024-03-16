using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NoteStash.Converters
{
    /// <summary>
    /// Converter to convert from <see cref="Visibility"/> to <see cref="GridLength"/> or what is deemed here as width.
    /// Primarily used for the `Diff` functionality where two text boxes are show side by side and share screen
    /// real estate equally. One of the text boxes is always visible and has width of 1*, while the other is toggled
    /// collapsed and visible. The only way for the second text box to actually become collapsed is to set its width to
    /// Auto as well as settings its visibility to collapsed.
    /// </summary>
    public class VisibilityToWidthConverter : IValueConverter
    {
        /// <summary>
        /// Converts from <see cref="Visibility"/> to <see cref="GridLength"/>. Should only convert to
        /// <see cref="GridLength.Auto"/> or a single <see cref="GridUnitType.Star"/>.
        /// </summary>
        /// <param name="value">A <see cref="Visibility"/> value.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>A <see cref="GridLength"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Collapsed ? GridLength.Auto : new GridLength(1, GridUnitType.Star);
        }

        /// <summary>
        /// Not implemented as there should be no conversion back.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}