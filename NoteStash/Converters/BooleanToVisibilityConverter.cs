using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NoteStash.Converters;

/// <summary>
/// Converter for converting between <see cref="bool"/> and <see cref="Visibility"/>. It only converts to either
/// <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/>. Primarily used along with
/// <see cref="bool"/> properties to determine whether a widget is enabled/visible or not.
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
	/// <summary>
	/// Converts <see cref="bool"/> to <see cref="Visibility"/>.
	/// </summary>
	/// <param name="value">A <see cref="bool"/> value.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns>A <see cref="Visibility"/> value.</returns>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (bool)value ? Visibility.Visible : Visibility.Collapsed;
	}

	/// <summary>
	/// Converts <see cref="Visibility"/> to <see cref="bool"/>.
	/// </summary>
	/// <param name="value">A <see cref="Visibility"/> value.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (Visibility)value == Visibility.Visible;
	}
}
