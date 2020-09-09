using System;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides helper methods for working with <see cref="byte"/>.
	/// </summary>
	public static class ByteUtility
	{
		/// <summary>
		/// Converts the specified string representation of bytes into a byte array.
		/// </summary>
		/// <param name="value">The byte values; these must be hexadecimal numbers with no padding or separators.</param>
		/// <returns>A byte array containing the bytes represented in the string.</returns>
		public static byte[] ToBytes(string value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value));
			if (value.Length % 2 != 0)
				throw new ArgumentException("There must be an even number of characters in the string.", nameof(value));

			byte[] bytes = new byte[value.Length / 2];
			for (var index = 0; index < bytes.Length; index++)
				bytes[index] = (byte) (ConvertToNibble(value[index * 2]) * 16 + ConvertToNibble(value[index * 2 + 1]));

			return bytes;
		}

		/// <summary>
		/// Converts the specified byte array to a string of hex digits.
		/// </summary>
		/// <param name="bytes">The byte array to convert.</param>
		/// <returns>A string containing two hexadecimal digits for each input byte.</returns>
		public static string ToString(byte[] bytes)
		{
			if (bytes is null)
				throw new ArgumentNullException(nameof(bytes));

			const string hexDigits = "0123456789ABCDEF";

			// allocate char array and fill it with high and low nibbles of each byte
			char[] chars = new char[bytes.Length * 2];
			for (var index = 0; index < chars.Length; index += 2)
			{
				var by = bytes[index / 2];
				chars[index] = hexDigits[by / 16];
				chars[index + 1] = hexDigits[by % 16];
			}

			return new string(chars);
		}

		private static int ConvertToNibble(char ch)
		{
			if (ch >= '0' && ch <= '9')
				return ch - '0';
			else if (ch >= 'A' && ch <= 'F')
				return ch - 'A' + 10;
			else if (ch >= 'a' && ch <= 'f')
				return ch - 'a' + 10;
			else
				throw new ArgumentOutOfRangeException(nameof(ch));
		}
	}
}
