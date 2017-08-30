using System;
using System.Globalization;

namespace Faithlife.Utility.Invariant
{
	/// <summary>
	/// Methods for converting to and from strings using the invariant culture.
	/// </summary>
	public static class InvariantConvert
	{
		/// <summary>
		/// Converts the value to "true" or "false".
		/// </summary>
		public static string ToInvariantString(this bool value)
		{
			return value ? "true" : "false";
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		public static bool? TryParseBoolean(string text)
		{
			bool value;
			return bool.TryParse(text, out value) ? value : default(bool?);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		/// <exception cref="FormatException">Failed to parse using invariant culture.</exception>
		public static bool ParseBoolean(string text)
		{
			return ThrowFormatExceptionIfNull(TryParseBoolean(text), text);
		}

		/// <summary>
		/// Converts the value to a string using the invariant culture.
		/// </summary>
		public static string ToInvariantString(this double value)
		{
			// XmlConvert.ToString supports negative zero
			if (value == 0.0 && BitConverter.DoubleToInt64Bits(value) == BitConverter.DoubleToInt64Bits(-0.0))
				return "-0";

			return value.ToString("R", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		public static double? TryParseDouble(string text)
		{
			double value;
			if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
			{
				// XmlConvert.ToDouble supports negative zero
				if (value == 0.0 && text.TrimStart()[0] == '-')
					return -0.0;

				return value;
			}

			// XmlConvert.ToString uses INF
			if (text == "INF")
				return double.PositiveInfinity;
			if (text == "-INF")
				return double.NegativeInfinity;

			return default(double?);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		/// <exception cref="FormatException">Failed to parse using invariant culture.</exception>
		public static double ParseDouble(string text)
		{
			return ThrowFormatExceptionIfNull(TryParseDouble(text), text);
		}

		/// <summary>
		/// Converts the value to a string using the invariant culture.
		/// </summary>
		public static string ToInvariantString(this int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		public static int? TryParseInt32(string text)
		{
			int value;
			return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : default(int?);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		/// <exception cref="FormatException">Failed to parse using invariant culture.</exception>
		public static int ParseInt32(string text)
		{
			return ThrowFormatExceptionIfNull(TryParseInt32(text), text);
		}

		/// <summary>
		/// Converts the value to a string using the invariant culture.
		/// </summary>
		public static string ToInvariantString(this long value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		public static long? TryParseInt64(string text)
		{
			long value;
			return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : default(long?);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		/// <exception cref="FormatException">Failed to parse using invariant culture.</exception>
		public static long ParseInt64(string text)
		{
			return ThrowFormatExceptionIfNull(TryParseInt64(text), text);
		}

		/// <summary>
		/// Converts the value to a string using the invariant culture.
		/// </summary>
		public static string ToInvariantString(this TimeSpan value)
		{
			return value.ToString("c", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		public static TimeSpan? TryParseTimeSpan(string text)
		{
			TimeSpan value;
			return TimeSpan.TryParseExact(text, "c", CultureInfo.InvariantCulture, out value) ? value : default(TimeSpan?);
		}

		/// <summary>
		/// Converts the string to a value using the invariant culture.
		/// </summary>
		/// <exception cref="FormatException">Failed to parse using invariant culture.</exception>
		public static TimeSpan ParseTimeSpan(string text)
		{
			return ThrowFormatExceptionIfNull(TryParseTimeSpan(text), text);
		}

		private static T ThrowFormatExceptionIfNull<T>(T? result, string text)
			where T : struct
		{
			if (!result.HasValue)
			{
				throw new FormatException("Failed to parse {0} using invariant culture: {1}"
					.FormatInvariant(typeof(T).Name, text ?? "(null)"));
			}
			return result.Value;
		}
	}
}
