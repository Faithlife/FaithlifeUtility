using System;
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
		/// Append the invariant representation of the specfied format tot he end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="strFormat">A composite format string.</param>
		/// <param name="args">An array of objects to format</param>
		/// <returns>The <see cref="StringBuilder"/> instance that was provided.</returns>
		public static StringBuilder AppendFormatInvariant(this StringBuilder sb, string strFormat, params object[] args)
		{
			return sb.AppendFormat(CultureInfo.InvariantCulture, strFormat, args);
		}

		/// <summary>
		/// Appends the invariant representation of a specified boolean value to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, bool value)
		{
			return sb.Append(value.ToString());
		}

		/// <summary>
		/// Appends the invariant representation of a specified 8-bit unsigned integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, byte value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified decimal number to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, decimal value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified double-precision floating-point number to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, double value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified 16-bit signed integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, short value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified 32-bit signed integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, int value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified 64-bit signed integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, long value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified single-precision floating-point number to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, float value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified 8-bit signed integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, sbyte value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified 16-bit unsigned integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, ushort value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified 32-bit unsigned integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, uint value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Appends the invariant representation of a specified 64-bit unsigned integer to the end of <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> instance to which the string representation will be appended.</param>
		/// <param name="value">The value to append.</param>
		public static StringBuilder AppendInvariant(this StringBuilder sb, ulong value)
		{
			return sb.Append(value.ToString(CultureInfo.InvariantCulture));
		}
	}
}
