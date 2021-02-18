using System;

namespace Faithlife.Utility
{
	/// <summary>
	///  Provides helper methods for working with enumerated values.
	/// </summary>
	public static class EnumUtility
	{
		/// <summary>
		/// Gets the values defined by the enumerated type.
		/// </summary>
		/// <typeparam name="T">The enumerated type.</typeparam>
		/// <returns>The values defined by the enumerated type, sorted by value.</returns>
		public static T[] GetValues<T>()
			where T : struct, Enum => (T[]) Enum.GetValues(typeof(T));

		/// <summary>
		/// Determines whether the specified value is defined.
		/// </summary>
		/// <typeparam name="T">The enumerated type.</typeparam>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the specified value is defined; otherwise, <c>false</c>.</returns>
		public static bool IsDefined<T>(T value)
			where T : struct, Enum => Enum.IsDefined(typeof(T), value!);

		/// <summary>
		/// Parses the specified string.
		/// </summary>
		/// <typeparam name="T">The enumerated value type.</typeparam>
		/// <param name="value">The string.</param>
		/// <returns>A strongly typed enumerated value.</returns>
		/// <remarks>This method matches case.</remarks>
		public static T Parse<T>(string value)
			where T : struct, Enum => Parse<T>(value, CaseSensitivity.MatchCase);

		/// <summary>
		/// Parses the specified string.
		/// </summary>
		/// <typeparam name="T">The enumerated value type.</typeparam>
		/// <param name="value">The string.</param>
		/// <param name="caseSensitivity">The case sensitivity.</param>
		/// <returns>A strongly typed enumerated value.</returns>
		public static T Parse<T>(string value, CaseSensitivity caseSensitivity)
			where T : struct, Enum => (T) Enum.Parse(typeof(T), value, caseSensitivity == CaseSensitivity.IgnoreCase);

		/// <summary>
		/// Attempts to parse the specified string.
		/// </summary>
		/// <typeparam name="T">The enumerated value type.</typeparam>
		/// <param name="value">The string.</param>
		/// <returns>A strongly typed enumerated value; or null if the string could not be successfully parsed.</returns>
		/// <remarks>This method matches case.</remarks>
		public static T? TryParse<T>(string? value)
			where T : struct, Enum => TryParse<T>(value, CaseSensitivity.MatchCase);

		/// <summary>
		/// Attempts to parse the specified string.
		/// </summary>
		/// <typeparam name="T">The enumerated value type.</typeparam>
		/// <param name="value">The string.</param>
		/// <param name="caseSensitivity">The case sensitivity.</param>
		/// <returns>A strongly typed enumerated value; null if the string could not be successfully parsed.</returns>
		public static T? TryParse<T>(string? value, CaseSensitivity caseSensitivity)
			where T : struct, Enum => Enum.TryParse<T>(value, caseSensitivity == CaseSensitivity.IgnoreCase, out var result) ? result : default(T?);

		/// <summary>
		/// Attempts to parse the specified string.
		/// </summary>
		/// <typeparam name="T">The enumerated value type.</typeparam>
		/// <param name="value">The string.</param>
		/// <param name="result">The resulting enumerated value.</param>
		/// <returns>True if the string was successfully parsed.</returns>
		/// <remarks>This method matches case.</remarks>
		public static bool TryParse<T>(string? value, out T result)
			where T : struct, Enum => TryParse(value, CaseSensitivity.MatchCase, out result);

		/// <summary>
		/// Attempts to parse the specified string.
		/// </summary>
		/// <typeparam name="T">The enumerated value type.</typeparam>
		/// <param name="value">The string.</param>
		/// <param name="caseSensitivity">The case sensitivity.</param>
		/// <param name="result">The resulting enumerated value.</param>
		/// <returns>True if the string was successfully parsed.</returns>
		/// <remarks>This method ignores case.</remarks>
		public static bool TryParse<T>(string? value, CaseSensitivity caseSensitivity, out T result)
			where T : struct, Enum => Enum.TryParse(value, caseSensitivity == CaseSensitivity.IgnoreCase, out result);
	}
}
