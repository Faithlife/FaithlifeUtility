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
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			if (value.Length % 2 != 0)
				throw new ArgumentException("There must be an even number of characters in the string.", nameof(value));

			byte[] bytes = new byte[value.Length / 2];
			for (int nByte = 0; nByte < bytes.Length; nByte++)
				bytes[nByte] = (byte) (ConvertToNibble(value[nByte * 2]) * 16 + ConvertToNibble(value[nByte * 2 + 1]));

			return bytes;
		}

		/// <summary>
		/// Converts the specified byte array to a string of hex digits.
		/// </summary>
		/// <param name="bytes">The byte array to convert.</param>
		/// <returns>A string containing two hexadecimal digits for each input byte.</returns>
		public static string ToString(byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes));

			const string c_strHexDigits = "0123456789ABCDEF";

			// allocate char array and fill it with high and low nibbles of each byte
			char[] chars = new char[bytes.Length * 2];
			for (int nIndex = 0; nIndex < chars.Length; nIndex += 2)
			{
				byte by = bytes[nIndex / 2];
				chars[nIndex] = c_strHexDigits[by / 16];
				chars[nIndex + 1] = c_strHexDigits[by % 16];
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
