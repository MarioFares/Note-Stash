using System;
using System.Globalization;
using System.Windows.Data;

namespace NoteStash.Converters;

/// <summary>
/// Converter for converting from a boolean vale to its inverse, i.e. if the input is true
/// false is returned and vice versa.
/// </summary>
public class BooleanToInverseBooleanConverter : IValueConverter
{
	/// <summary>
	/// Converts from a boolean to its negation.
	/// </summary>
	/// <param name="value">The boolean value to negate.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns>Returns false if the input is true and true if the input is false.</returns>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return !(bool)value;
	}

	/// <summary>
	/// Converts from a boolean to its negation.
	/// </summary>
	/// <param name="value">The boolean value to negate.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns>Returns false if the input is true and true if the input is false.</returns>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return !(bool)value;
	}
}