using System;
using System.Globalization;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating dates.
	/// </summary>
	public static class DateTimeUtility
	{
		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTime equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <returns>The DateTime equivalent.</returns>
		public static DateTime ParseIso8601(string value)
			=> DateTime.ParseExact(value, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTime equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <param name="date">The DateTime equivalent.</param>
		/// <returns>True if successful.</returns>
		public static bool TryParseIso8601(string value, out DateTime date)
			=> DateTime.TryParseExact(value, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out date);

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTime equivalent.
		/// </summary>
		/// <param name="value">The ISO 8601 string representation to parse.</param>
		/// <returns>Non-null if successful.</returns>
		public static DateTime? TryParseIso8601(string value) => TryParseIso8601(value, out var dt) ? dt : default(DateTime?);

		/// <summary>
		/// Formats the date in the standard ISO 8601 format.
		/// </summary>
		/// <param name="date">The date to format.</param>
		/// <returns>The formatted date.</returns>
		public static string ToIso8601(this DateTime date) => date.ToUniversalTime().ToString(Iso8601Format, CultureInfo.InvariantCulture);

		/// <summary>
		/// The ISO 8601 format string.
		/// </summary>
#if !MAC
		public const string Iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
#else
		// Workaround for http://bugzilla.novell.com/show_bug.cgi?id=403409
		public const string Iso8601Format = "yyyy-MM-ddTHH:mm:ssZ";
#endif

		/// <summary>
		/// The number of DateTime ticks in one second.
		/// </summary>
		public const long TicksPerSecond = 10000000L;

		/// <summary>
		/// Returns the converted DateTime, in seconds since Unix epoch.
		/// </summary>
		/// <param name="date">The UTC DateTime requiring conversion.</param>
		/// <returns>Returns the number of seconds the provided UTC DateTime is greater than the Unix epoch (1/1/1970).</returns>
		public static long ToUnixTimestamp(DateTime date)
		{
			if (date.Kind != DateTimeKind.Utc)
				throw new ArgumentException("Specified DateTime value must be UTC.", nameof(date));

			return (long) (date - s_epoch).TotalSeconds;
		}

		/// <summary>
		/// Returns the converted UnixTimestamp in a DateTime.
		/// </summary>
		/// <param name="unixTimestamp">The UnixTimestamp, in seconds from epoch.</param>
		/// <returns>Returns the DateTime representation of the UnixTimestamp.</returns>
		public static DateTime FromUnixTimestamp(long unixTimestamp) => s_epoch.AddSeconds(unixTimestamp);

		/// <summary>
		/// Returns true if the two date times are equal when rounded down to the second.
		/// </summary>
		/// <remarks>Use this method to compare date times that are serialized with a format
		/// that doesn't preserve sub-seconds.</remarks>
		public static bool AreSameSecond(DateTime left, DateTime right) => left.Ticks / TicksPerSecond == right.Ticks / TicksPerSecond;

		/// <summary>
		/// Returns true if the two date times are equal when rounded down to the second.
		/// </summary>
		/// <remarks>Use this method to compare date times that are serialized with a format
		/// that doesn't preserve sub-seconds.</remarks>
		public static bool AreSameSecond(DateTime? left, DateTime? right)
			=> left.HasValue && right.HasValue ? AreSameSecond(left.Value, right.Value) : left.HasValue == right.HasValue;

		/// <summary>
		/// Specifies DateTimeKind.Utc.
		/// </summary>
		public static DateTime SpecifyUtc(this DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

		/// <summary>
		/// Specifies DateTimeKind.Utc if not null.
		/// </summary>
		public static DateTime? SpecifyUtc(this DateTime? dateTime) => dateTime?.SpecifyUtc();

		private static readonly DateTime s_epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
