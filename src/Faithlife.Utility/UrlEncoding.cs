using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Faithlife.Utility
{
	/// <summary>
	/// Methods for encoding and decoding URL-style strings.
	/// </summary>
	public static class UrlEncoding
	{
		/// <summary>
		/// Encodes a string with the default settings.
		/// </summary>
		/// <param name="value">The string to encode.</param>
		/// <returns>The encoded string.</returns>
		[return: NotNullIfNotNull("value")]
		public static string? Encode(string? value) => Encode(value, s_settingsDefault);

		/// <summary>
		/// Encodes a string with the specified settings.
		/// </summary>
		/// <param name="value">The string to encode.</param>
		/// <param name="settings">The settings to use when encoding.</param>
		/// <returns>The encoded string.</returns>
		[return: NotNullIfNotNull("value")]
		public static string? Encode(string? value, UrlEncodingSettings settings)
		{
			// check arguments
			if (settings is null)
				throw new ArgumentNullException(nameof(settings));

			// null encodes to null
			if (value is null)
				return null;

			// empty string encodes to empty string
			if (value.Length == 0)
				return value;

			// convert string to array of characters
			char[] chars = value.ToCharArray();
			int length = chars.Length;

			// count characters that should be encoded and spaces
			int charsToEncode = 0;
			for (int index = 0; index < length; index++)
			{
				if (ShouldEncodeChar(settings, chars, index))
					charsToEncode++;
			}

			// we're done if there are no characters to encode
			if (charsToEncode == 0)
				return value;

			// each byte becomes 3 characters
			Encoding encoding = settings.TextEncoding;
			char[] output = new char[length + 3 * settings.TextEncoding.GetMaxByteCount(charsToEncode)];
			int outputChar = 0;

			// walk characters
			char? encodedSpaceChar = settings.EncodedSpaceChar;
			char encodedBytePrefixChar = settings.EncodedBytePrefixChar;
			int encodeStart = 0;
			int encodeLength = 0;
			for (int index = 0; index < length; index++)
			{
				// determine if character needs to be encoded
				bool shouldEncode = ShouldEncodeChar(settings, chars, index);
				bool encodingSpaceChar = shouldEncode && chars[index] == ' ' && encodedSpaceChar.HasValue;

				// determine if the next character doesn't need text encoding
				if (!shouldEncode || encodingSpaceChar)
				{
					// encode any characters that needed text encoding
					if (encodeLength != 0)
					{
						foreach (byte by in encoding.GetBytes(chars, encodeStart, encodeLength))
						{
							output[outputChar++] = encodedBytePrefixChar;
							output[outputChar++] = HexChar((by >> 4) & 0xf, settings);
							output[outputChar++] = HexChar(by & 0xf, settings);
						}

						encodeLength = 0;
					}
				}

				if (encodingSpaceChar)
				{
					// encode space character directly
					output[outputChar++] = encodedSpaceChar!.Value;
				}
				else if (shouldEncode)
				{
					// start run of characters that need to be encoded
					if (encodeLength == 0)
						encodeStart = index;
					encodeLength++;
				}
				else
				{
					// copy character to destination
					output[outputChar++] = chars[index];
				}
			}

			// encode any characters that needed text encoding
			if (encodeLength != 0)
			{
				foreach (byte by in encoding.GetBytes(chars, encodeStart, encodeLength))
				{
					output[outputChar++] = encodedBytePrefixChar;
					output[outputChar++] = HexChar((by >> 4) & 0xf, settings);
					output[outputChar++] = HexChar(by & 0xf, settings);
				}
			}

			// create new string from array of characters
			return new string(output, 0, outputChar);
		}

		/// <summary>
		/// Decodes a string with the default settings.
		/// </summary>
		/// <param name="value">The string to be decoded.</param>
		/// <returns>The decoded string.</returns>
		[return: NotNullIfNotNull("value")]
		public static string? Decode(string? value) => Decode(value, s_settingsDefault);

		/// <summary>
		/// Decodes a string with the specified settings.
		/// </summary>
		/// <param name="value">The string to be decoded.</param>
		/// <param name="settings">The settings to use when decoding.</param>
		/// <returns>The decoded string.</returns>
		[return: NotNullIfNotNull("value")]
		public static string? Decode(string? value, UrlEncodingSettings settings)
		{
			// check arguments
			if (settings is null)
				throw new ArgumentNullException(nameof(settings));

			// null decodes to null
			if (value is null)
				return null;

			// empty string decodes to empty string
			if (value.Length == 0)
				return value;

			// replace encoded spaces if necessary
			if (settings.EncodedSpaceChar.HasValue)
				value = value.Replace(settings.EncodedSpaceChar.Value, ' ');

			// decode hex-encoded characters
			return value.IndexOf(settings.EncodedBytePrefixChar) == -1 ? value : DecodeHex(value, settings);
		}

		private static string DecodeHex(string str, UrlEncodingSettings settings)
		{
			var output = new char[str.Length];
			var outputIndex = 0;
			var singleByteArray = new byte[1];
			var decoder = settings.TextEncoding.GetDecoder();
			var isUtf8 = object.ReferenceEquals(settings.TextEncoding, Encoding.UTF8);

			// walk the string, looking for the prefix character
			for (int index = 0; index < str.Length; index++)
			{
				char ch = str[index];
				if (index < str.Length - 2 && ch == settings.EncodedBytePrefixChar && IsCharHex(str[index + 1]) && IsCharHex(str[index + 2]))
				{
					// found two hex characters; add their byte value to the buffer
					var value = unchecked((byte) ((CharHex(str[index + 1]) << 4) + CharHex(str[index + 2])));
					index += 2;

					if (isUtf8 && value <= 0x7F)
					{
						// this byte converts straight to the equivalent char
						output[outputIndex++] = (char) value;
					}
					else
					{
						// use the decoder to decode this byte
						singleByteArray[0] = value;
						outputIndex += decoder.GetChars(singleByteArray, 0, 1, output, outputIndex);
					}
				}
				else
				{
					// decode the buffer first if it contains data
					if (singleByteArray[0] != 0)
					{
						outputIndex += decoder.GetChars(singleByteArray, 0, 0, output, outputIndex, flush: true);
						singleByteArray[0] = 0;
					}

					// copy this character as-is
					output[outputIndex++] = ch;
				}
			}

			// decode any remaining bytes in the buffer
			if (singleByteArray[0] != 0)
				outputIndex += decoder.GetChars(singleByteArray, 0, 0, output, outputIndex, flush: true);

			return new string(output, 0, outputIndex);
		}

		private static bool IsCharHex(char ch)
		{
			return (ch >= '0' && ch <= '9') ||
				(ch >= 'a' && ch <= 'f') ||
				(ch >= 'A' && ch <= 'F');
		}

		private static int CharHex(char ch)
		{
			Debug.Assert(IsCharHex(ch), "IsCharHex(ch)");

			// convert hex nybble to integer ('1' == 1, A == '10', etc.)
			if (ch >= '0' && ch <= '9')
				return ch - '0';
			else if (ch >= 'a' && ch <= 'f')
				return ch - 'a' + 10;
			else
				return ch - 'A' + 10;
		}

		private static char HexChar(int n, UrlEncodingSettings settings)
		{
			// convert integer to hex nybble
			return (char) (n < 10 ? n + '0' : n - 10 + (settings.UppercaseHexDigits ? 'A' : 'a'));
		}

		private static bool ShouldEncodeChar(UrlEncodingSettings settings, char[] chars, int index)
		{
			char ch = chars[index];

			// the char should be encoded if the user-supplied function indicates it should
			if (settings.ShouldEncodeChar(ch))
				return true;

			// the char should be encoded if it's the char used to encode spaces
			if (settings.EncodedSpaceChar.HasValue && ch == settings.EncodedSpaceChar.Value)
				return true;

			// the char should be encoded if it's a space (and spaces have custom encodings)
			if (settings.EncodedSpaceChar.HasValue && ch == ' ')
				return true;

			// the char should be encoded if it's the char used to encode other characters
			return ch == settings.EncodedBytePrefixChar &&
				!(settings.PreventDoubleEncoding && index + 2 < chars.Length && IsCharHex(chars[index + 1]) && IsCharHex(chars[index + 2]));
		}

		static readonly UrlEncodingSettings s_settingsDefault = new UrlEncodingSettings();
	}
}
