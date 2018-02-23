using System.Globalization;
using System.Text;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating <see cref="StringBuilder"/> objects.
	/// </summary>
	public static class StringBuilderUtility
	{
		/// <summary>
		/// Append the invariant representation of the specfied format to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An array of objects to format</param>
		/// <returns>The <see cref="StringBuilder"/> instance that was provided.</returns>
		public static StringBuilder AppendFormatInvariant(this StringBuilder stringBuilder, string format, params object[] args)
			=> stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, args);

		/// <summary>
		/// Appends the invariant representation of a specified boolean value to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, bool value)
			=> stringBuilder.Append(value.ToString());

		/// <summary>
		/// Appends the invariant representation of a specified 8-bit unsigned integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, byte value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified decimal number to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, decimal value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified double-precision floating-point number to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, double value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified 16-bit signed integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, short value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified 32-bit signed integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, int value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified 64-bit signed integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, long value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified single-precision floating-point number to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, float value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified 8-bit signed integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, sbyte value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified 16-bit unsigned integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, ushort value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified 32-bit unsigned integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, uint value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));

		/// <summary>
		/// Appends the invariant representation of a specified 64-bit unsigned integer to the end of <paramref name="stringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder stringBuilder, ulong value)
			=> stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
	}
}
