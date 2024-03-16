using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NoteStash.Converters;

/// <summary>
/// Converter to convert from <see cref="int"/> to <see cref="Visibility"/>. It should only convert to either
/// <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/>. Primarily used along with
/// <see cref="int"/> properties that are bound to groups of MenuItems where in that group, there is at least one
/// option to turn off the functionality in question while the rest are there to customize it.
/// </summary>
public class IntToVisibilityConverter : IValueConverter
{
	/// <summary>
	/// Converts from <see cref="int"/> to <see cref="Visibility"/>, and should only convert to either
	/// <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/>. An integer value of 0
	/// should correspond to <see cref="Visibility.Collapsed"/> whereas anything else should be
	/// <see cref="Visibility.Visible"/>.
	/// </summary>
	/// <param name="value">An <see cref="int"/> value.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns>A <see cref="Visibility"/> value.</returns>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
	}

	/// <summary>
	/// Not implemented as there should no need to convert back from <see cref="Visibility"/> to <see cref="int"/>.
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
