using System;
using System.Globalization;
using System.Windows.Data;

namespace NoteStash.Converters;

/// <summary>
/// Converter for converting from <see cref="DateTime"/> to <see cref="string"/>. Primarily used along with
/// <see cref="DateTime"/> properties such as file access and creation dates that are converted to strings
/// for display. Since they might be set to default, the converter is used for all of them to either return
/// the datetime as a string or return an empty string if the datetime is the default value.
/// </summary>
public class DateTimeToStringConverter : IValueConverter
{
	/// <summary>
	/// Converts <see cref="DateTime"/> to <see cref="string"/>.
	/// </summary>
	/// <param name="value">A <see cref="DateTime"/> value.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns>A <see cref="string"/> value.</returns>
	/// <remarks>Uses current culture.</remarks>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var dateTime = (DateTime)value;
		return dateTime == default ? string.Empty : dateTime.ToString(CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Not implemented as there should be no need to convert back from a <see cref="string"/> to a
	/// <see cref="DateTime"/>.
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
