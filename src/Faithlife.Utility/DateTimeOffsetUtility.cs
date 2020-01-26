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
			=> DateTimeOffset.ParseExact(value, GetParseFormat(value), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTimeOffset equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <param name="date">The DateTimeOffset equivalent.</param>
		/// <returns>True if successful.</returns>
		public static bool TryParseIso8601(string? value, out DateTimeOffset date)
			=> DateTimeOffset.TryParseExact(value, GetParseFormat(value), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date);

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTimeOffset equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <returns>Non-null if successful.</returns>
		public static DateTimeOffset? TryParseIso8601(string? value) => TryParseIso8601(value, out var dt) ? dt : default(DateTimeOffset?);

		/// <summary>
		/// Formats the date in the standard ISO 8601 format.
		/// </summary>
		/// <param name="date">The date to format.</param>
		/// <returns>The formatted date.</returns>
		public static string ToIso8601(this DateTimeOffset date) => date.ToString(Iso8601Format, CultureInfo.InvariantCulture);

		/// <summary>
		/// The ISO 8601 format string.
		/// </summary>
		public const string Iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

		// Returns a string containing the pattern that should be used for parsing the input.
		private static string GetParseFormat(string? strInput)
		{
			// a trailing "Z" means to use the "legacy" timezone format
			return strInput is object && strInput.EndsWith("Z", StringComparison.Ordinal) ? DateTimeUtility.Iso8601Format : Iso8601Format;
		}
	}
}
