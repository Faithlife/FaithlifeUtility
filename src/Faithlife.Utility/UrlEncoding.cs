using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		/// <param name="str">The string to encode.</param>
		/// <returns>The encoded string.</returns>
		public static string Encode(string str)
		{
			// use default settings
			return Encode(str, s_settingsDefault);
		}

		/// <summary>
		/// Encodes a string with the specified settings.
		/// </summary>
		/// <param name="str">The string to encode.</param>
		/// <param name="settings">The settings to use when encoding.</param>
		/// <returns>The encoded string.</returns>
		public static string Encode(string str, UrlEncodingSettings settings)
		{
			// check arguments
			if (settings == null)
				throw new ArgumentNullException("settings");

			// null encodes to null
			if (str == null)
				return null;

			// empty string encodes to empty string
			if (str.Length == 0)
				return str;

			// convert string to array of characters
			char[] achChars = str.ToCharArray();
			int nChars = achChars.Length;

			// count characters that should be encoded and spaces
			int nCharsToEncode = 0;
			for (int nChar = 0; nChar < nChars; nChar++)
			{
				if (ShouldEncodeChar(settings, achChars, nChar))
					nCharsToEncode++;
			}

			// we're done if there are no characters to encode
			if (nCharsToEncode == 0)
				return str;

			// each byte becomes 3 characters
			Encoding encoding = settings.TextEncoding;
			char[] achOutput = new char[nChars + 3 * settings.TextEncoding.GetMaxByteCount(nCharsToEncode)];
			int nOutputChar = 0;

			// walk characters
			char? chEncodedSpaceChar = settings.EncodedSpaceChar;
			char chEncodedBytePrefixChar = settings.EncodedBytePrefixChar;
			int nEncodeStart = 0;
			int nEncodeLength = 0;
			for (int nChar = 0; nChar < nChars; nChar++)
			{
				// determine if character needs to be encoded
				bool bShouldEncode = ShouldEncodeChar(settings, achChars, nChar);
				bool bEncodedSpaceChar = bShouldEncode && achChars[nChar] == ' ' && chEncodedSpaceChar.HasValue;

				// determine if the next character doesn't need text encoding
				if (!bShouldEncode || bEncodedSpaceChar)
				{
					// encode any characters that needed text encoding
					if (nEncodeLength != 0)
					{
						foreach (byte by in encoding.GetBytes(achChars, nEncodeStart, nEncodeLength))
						{
							achOutput[nOutputChar++] = chEncodedBytePrefixChar;
							achOutput[nOutputChar++] = HexChar((by >> 4) & 0xf, settings);
							achOutput[nOutputChar++] = HexChar(by & 0xf, settings);
						}

						nEncodeLength = 0;
					}
				}

				if (bEncodedSpaceChar)
				{
					// encode space character directly
					achOutput[nOutputChar++] = chEncodedSpaceChar.Value;
				}
				else if (bShouldEncode)
				{
					// start run of characters that need to be encoded
					if (nEncodeLength == 0)
						nEncodeStart = nChar;
					nEncodeLength++;
				}
				else
				{
					// copy character to destination
					achOutput[nOutputChar++] = achChars[nChar];
				}
			}

			// encode any characters that needed text encoding
			if (nEncodeLength != 0)
			{
				foreach (byte by in encoding.GetBytes(achChars, nEncodeStart, nEncodeLength))
				{
					achOutput[nOutputChar++] = chEncodedBytePrefixChar;
					achOutput[nOutputChar++] = HexChar((by >> 4) & 0xf, settings);
					achOutput[nOutputChar++] = HexChar(by & 0xf, settings);
				}
			}

			// create new string from array of characters
			return new string(achOutput, 0, nOutputChar);
		}

		/// <summary>
		/// Decodes a string with the default settings.
		/// </summary>
		/// <param name="str">The string to be decoded.</param>
		/// <returns>The decoded string.</returns>
		public static string Decode(string str)
		{
			return Decode(str, s_settingsDefault);
		}

		/// <summary>
		/// Decodes a string with the specified settings.
		/// </summary>
		/// <param name="str">The string to be decoded.</param>
		/// <param name="settings">The settings to use when decoding.</param>
		/// <returns>The decoded string.</returns>
		public static string Decode(string str, UrlEncodingSettings settings)
		{
			// check arguments
			if (settings == null)
				throw new ArgumentNullException("settings");

			// null decodes to null
			if (str == null)
				return null;

			// empty string decodes to empty string
			if (str.Length == 0)
				return str;

			// replace encoded spaces if necessary
			if (settings.EncodedSpaceChar.HasValue)
				str = str.Replace(settings.EncodedSpaceChar.Value, ' ');

			// decode hex-encoded characters
			return str.IndexOf(settings.EncodedBytePrefixChar) == -1 ? str : DecodeHex(str, settings);
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

		private static bool ShouldEncodeChar(UrlEncodingSettings settings, char[] achChars, int nChar)
		{
			char ch = achChars[nChar];

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
				!(settings.PreventDoubleEncoding && nChar + 2 < achChars.Length && IsCharHex(achChars[nChar + 1]) && IsCharHex(achChars[nChar + 2]));
		}

		static readonly UrlEncodingSettings s_settingsDefault = new UrlEncodingSettings();
	}
}
