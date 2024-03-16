using System;
using System.Globalization;
using System.Windows.Data;

namespace NoteStash.Converters;

/// <summary>
/// Converter to convert from longs representing file sizes in bytes to descriptive strings with value and unit.
/// </summary>
public class LongToSizeStringConverter : IValueConverter
{
	/// <summary>
	/// Converts from <see cref="long"/> to <see cref="string"/> that contains a numerical value for the size
	/// of the file that was passed as input and the units of the size. 
	/// </summary>
	/// <param name="value">A positive long that is measured in bytes.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter"></param>
	/// <param name="culture"></param>
	/// <returns>A string with an integer value and units used to measure file sizes.</returns>
	/// <remarks>Units include B, KB, MB, GB only.</remarks>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		long input = (long)value;
		return input switch
		{
			<= 0 => "0 B",
			< 1024 => $"{input} B",
			< 1024 * 1024 => $"{input / 1024} KB",
			< 1024 * 1024 * 1024 => $"{input / (1024 * 1024)} MB",
			_ => $"{input / (1024 * 1024 * 1024)} GB"
		};
	}

	/// <summary>
	/// Not implemented as there should be no need to convert back from <see cref="string"/> to <see cref="long"/>.
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