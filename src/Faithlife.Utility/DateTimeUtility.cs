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
		/// <param name="strValue">The ISO 8601 string representation to parse.</param>
		/// <returns>The DateTime equivalent.</returns>
		public static DateTime ParseIso8601(string strValue)
		{
			return DateTime.ParseExact(strValue,
				Iso8601Format, CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
		}

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTime equivalent.
		/// </summary>
		/// <param name="strValue">The ISO 8601 string representation to parse.</param>
		/// <param name="dt">The DateTime equivalent.</param>
		/// <returns>True if successful.</returns>
		public static bool TryParseIso8601(string strValue, out DateTime dt)
		{
			return DateTime.TryParseExact(strValue,
				Iso8601Format, CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dt);
		}

		/// <summary>
		/// Converts the specified ISO 8601 representation of a date and time to its DateTime equivalent.
		/// </summary>
		/// <param name="strValue">The ISO 8601 string representation to parse.</param>
		/// <returns>Non-null if successful.</returns>
		public static DateTime? TryParseIso8601(string strValue)
		{
			DateTime dt;
			return TryParseIso8601(strValue, out dt) ? dt : default(DateTime?);
		}

		/// <summary>
		/// Formats the date in the standard ISO 8601 format.
		/// </summary>
		/// <param name="dtValue">The date to format.</param>
		/// <returns>The formatted date.</returns>
		public static string RenderIso8601(DateTime dtValue)
		{
			return dtValue.ToUniversalTime().ToString(Iso8601Format, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Formats the date in the standard ISO 8601 format.
		/// </summary>
		/// <param name="dtValue">The date to format.</param>
		/// <returns>The formatted date.</returns>
		public static string ToIso8601(this DateTime dtValue)
		{
			return dtValue.ToUniversalTime().ToString(Iso8601Format, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Returns the maximum of the two provided date times.
		/// </summary>
		/// <param name="date1">The first date.</param>
		/// <param name="date2">The second date.</param>
		/// <returns>The first date time if it is greater; otherwise, the second.</returns>
		public static DateTime Max(DateTime date1, DateTime date2)
		{
			return date1 > date2 ? date1 : date2;
		}

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
		/// <param name="dtValue">The UTC DateTime requiring conversion.</param>
		/// <returns>Returns the number of seconds the provided UTC DateTime is greater than the Unix epoch (1/1/1970).</returns>
		public static long ToUnixTimestamp(DateTime dtValue)
		{
			if (dtValue.Kind != DateTimeKind.Utc)
				throw new ArgumentException("Specified DateTime value must be UTC.");

			return (long)(dtValue - s_epoch).TotalSeconds;
		}

		/// <summary>
		/// Returns the converted UnixTimestamp in a DateTime.
		/// </summary>
		/// <param name="unixTimestamp">The UnixTimestamp, in seconds from epoch.</param>
		/// <returns>Returns the DateTime representation of the UnixTimestamp.</returns>
		public static DateTime FromUnixTimestamp(long unixTimestamp)
		{
			return s_epoch.AddSeconds(unixTimestamp);
		}

		/// <summary>
		/// Returns true if the two date times are equal when rounded down to the second.
		/// </summary>
		/// <remarks>Use this method to compare date times that are serialized with a format
		/// that doesn't preserve sub-seconds.</remarks>
		public static bool AreSameSecond(DateTime left, DateTime right)
		{
			return left.Ticks / TicksPerSecond == right.Ticks / TicksPerSecond;
		}

		/// <summary>
		/// Returns true if the two date times are equal when rounded down to the second.
		/// </summary>
		/// <remarks>Use this method to compare date times that are serialized with a format
		/// that doesn't preserve sub-seconds.</remarks>
		public static bool AreSameSecond(DateTime? left, DateTime? right)
		{
			return (left.HasValue && right.HasValue) ? AreSameSecond(left.Value, right.Value) : (left.HasValue == right.HasValue);
		}

		/// <summary>
		/// Specifies DateTimeKind.Utc.
		/// </summary>
		public static DateTime SpecifyUtc(this DateTime dateTime)
		{
			return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
		}

		/// <summary>
		/// Specifies DateTimeKind.Utc if not null.
		/// </summary>
		public static DateTime? SpecifyUtc(this DateTime? dateTime)
		{
			return dateTime != null ? dateTime.Value.SpecifyUtc() : default(DateTime?);
		}

		private static readonly DateTime s_epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}