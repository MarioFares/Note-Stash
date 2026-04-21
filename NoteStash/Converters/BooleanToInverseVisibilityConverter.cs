using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NoteStash.Converters;

/// <summary>
/// Converter for converting between <see cref="bool"/> and <see cref="Visibility"/> with inverse logic.
/// Shows when bool is false, hides when bool is true.
/// </summary>
public class BooleanToInverseVisibilityConverter : IValueConverter
{
	/// <summary>
	/// Converts <see cref="bool"/> to <see cref="Visibility"/>.
	/// </summary>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (bool)value ? Visibility.Collapsed : Visibility.Visible;
	}

	/// <summary>
	/// Converts <see cref="Visibility"/> to <see cref="bool"/>.
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (Visibility)value != Visibility.Visible;
	}
}
