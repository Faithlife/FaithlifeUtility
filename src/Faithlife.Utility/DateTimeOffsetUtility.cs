using System;
using System.Globalization;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating dates.
	/// </summary>
	public static class DateTimeOffsetUtility
	{
		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTimeOffset equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <returns>The DateTimeOffset equivalent.</returns>
		public static DateTimeOffset ParseIso8601(string value)
		{
			return DateTimeOffset.ParseExact(value, GetParseFormat(value), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
		}

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTimeOffset equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <param name="date">The DateTimeOffset equivalent.</param>
		/// <returns>True if successful.</returns>
		public static bool TryParseIso8601(string value, out DateTimeOffset date)
		{
			return DateTimeOffset.TryParseExact(value, GetParseFormat(value), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date);
		}

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTimeOffset equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <returns>Non-null if successful.</returns>
		public static DateTimeOffset? TryParseIso8601(string value)
		{
			DateTimeOffset dt;
			return TryParseIso8601(value, out dt) ? dt : default(DateTimeOffset?);
		}

		/// <summary>
		/// Formats the date in the standard ISO 8601 format.
		/// </summary>
		/// <param name="date">The date to format.</param>
		/// <returns>The formatted date.</returns>
		public static string ToIso8601(this DateTimeOffset date)
		{
			return date.ToString(Iso8601Format, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Returns the larger of two DateTimeOffset instances.
		/// </summary>
		/// <param name="date1">The first of two dates to compare.</param>
		/// <param name="date2">The second of two dates to compare.</param>
		/// <returns>The larger of date1 or date2.</returns>
		public static DateTimeOffset Max(DateTimeOffset date1, DateTimeOffset date2)
		{
			return date1 > date2 ? date1 : date2;
		}

		/// <summary>
		/// Returns the smaller of two DateTimeOffset instances.
		/// </summary>
		/// <param name="date1">The first of two dates to compare.</param>
		/// <param name="date2">The second of two dates to compare.</param>
		/// <returns>The smaller of date1 or date2.</returns>
		public static DateTimeOffset Min(DateTimeOffset date1, DateTimeOffset date2)
		{
			return date1 < date2 ? date1 : date2;
		}

		/// <summary>
		/// The ISO 8601 format string.
		/// </summary>
		public const string Iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

		// Returns a string containing the pattern that should be used for parsing the input.
		private static string GetParseFormat(string strInput)
		{
			// a trailing "Z" means to use the "legacy" timezone format
			return strInput != null && strInput.EndsWith("Z", StringComparison.Ordinal) ? DateTimeUtility.Iso8601Format : Iso8601Format;
		}
	}
}
