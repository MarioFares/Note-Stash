using System;
using System.Globalization;
using System.Windows.Data;

namespace NoteStash.Converters;

/// <summary>
/// Converter to convert from <see cref="int"/> to <see cref="bool"/>. Primarily used along with
/// <see cref="int"/> properties that are bound to groups of MenuItems where in that group, there is at least one
/// option to turn off the functionality in question while the rest are there to customize it. The purpose of
/// this converter is to ensure that only when MenuItem out of the group is checked at a time.
/// </summary>
public class MatchingIntToBooleanConverter : IValueConverter
{
	/// <summary>
	/// Converts from a string parameter to a boolean value. Matches parameter against value and  returns false if
	/// not matched.
	/// </summary>
	/// <param name="value">An <see cref="int"/> value that is supposed to represented the current option.</param>
	/// <param name="targetType"></param>
	/// <param name="parameter">A <see cref="string"/> value that is supposed to represent the value of
	/// other options.
	/// </param>
	/// <param name="culture"></param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var paramVal = parameter as string;
		var objVal = ((int)value).ToString();

		return paramVal == objVal;
	}

	/// <summary>
	/// Converts the <see cref="bool"/> value to an <see cref="int"/>. value represents
	/// whether the MenuItem is checked or not. If the MenuItem is checked, then the ConverterParameter assigned
	/// to it will be returned.
	/// </summary>
	/// <param name="value">A <see cref="bool"/> value. Represents IsChecked property of MenuItems.</param>
	/// <param name="targetType">The target type for conversion, which must be an <see cref="int"/>.</param>
	/// <param name="parameter">A <see cref="string"/> value that is the ConverterParameter of each MenuItem.
	/// </param>
	/// <param name="culture"></param>
	/// <returns></returns>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool isChecked)
		{
			var i = System.Convert.ToInt32((parameter) as string);
			return isChecked ? System.Convert.ChangeType(i, targetType) : 0;
		}

		return 0; // Returning a zero provides a case where none of the MenuItems appear checked
	}
}