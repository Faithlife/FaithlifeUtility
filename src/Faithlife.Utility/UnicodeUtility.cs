using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace Faithlife.Utility
{
	/// <summary>
	/// Utility methods for working with Unicode characters.
	/// </summary>
	public static class UnicodeUtility
	{
		/// <summary>
		/// Returns the <see cref="UnicodeCharacterClass"/> of the character at the specified offset in the given string.
		/// </summary>
		/// <param name="value">A <see cref="string"/>.</param>
		/// <param name="index">The character position in <paramref name="value"/>.</param>
		/// <returns>A <see cref="UnicodeCharacterClass"/> enumerated constant that identifies the character class of the character at position <paramref name="index"/> in <paramref name="value"/>.</returns>
		public static UnicodeCharacterClass GetCharacterClass(string value, int index)
		{
			return GetCharacterClassFromCategory(CharUnicodeInfo.GetUnicodeCategory(value, index));
		}

		/// <summary>
		/// Returns the major character class for the specified Unicode general category.
		/// </summary>
		/// <param name="category">The <see cref="UnicodeCategory"/> of a character.</param>
		/// <returns>The class of the specified category.</returns>
		public static UnicodeCharacterClass GetCharacterClassFromCategory(UnicodeCategory category)
		{
			if (category == UnicodeCategory.UppercaseLetter || category == UnicodeCategory.LowercaseLetter || category == UnicodeCategory.TitlecaseLetter ||
				category == UnicodeCategory.ModifierLetter || category == UnicodeCategory.OtherLetter)
				return UnicodeCharacterClass.Letter;
			else if (category == UnicodeCategory.NonSpacingMark || category == UnicodeCategory.SpacingCombiningMark || category == UnicodeCategory.EnclosingMark)
				return UnicodeCharacterClass.Mark;
			else if (category == UnicodeCategory.DecimalDigitNumber || category == UnicodeCategory.LetterNumber || category == UnicodeCategory.OtherNumber)
				return UnicodeCharacterClass.Number;
			else if (category == UnicodeCategory.SpaceSeparator || category == UnicodeCategory.LineSeparator || category == UnicodeCategory.ParagraphSeparator)
				return UnicodeCharacterClass.Separator;
			else if (category == UnicodeCategory.ConnectorPunctuation || category == UnicodeCategory.DashPunctuation || category == UnicodeCategory.OpenPunctuation ||
				category == UnicodeCategory.ClosePunctuation || category == UnicodeCategory.InitialQuotePunctuation || category == UnicodeCategory.FinalQuotePunctuation ||
				category == UnicodeCategory.OtherPunctuation)
				return UnicodeCharacterClass.Punctuation;
			else if (category == UnicodeCategory.MathSymbol || category == UnicodeCategory.CurrencySymbol || category == UnicodeCategory.ModifierSymbol ||
				category == UnicodeCategory.OtherSymbol)
				return UnicodeCharacterClass.Symbol;
			else
				return UnicodeCharacterClass.Other;
		}

		/// <summary>
		/// Gets the length (in chars) of the Unicode character at the specified offset in the given string.
		/// </summary>
		/// <param name="value">A <see cref="string"/>.</param>
		/// <param name="index">The character position in <paramref name="value"/>.</param>
		/// <returns>The number of chars (i.e., UTF-16 code units) it takes to encode the character.</returns>
		public static int GetCharacterLength(string value, int index)
		{
			return char.IsSurrogatePair(value, index) ? 2 : 1;
		}

		/// <summary>
		/// Gets the length (in chars) of the Unicode character at the specified offset in the given <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="stringBuilder">A <see cref="StringBuilder"/>.</param>
		/// <param name="index">The character position in <paramref name="stringBuilder"/>.</param>
		/// <returns>The number of chars (i.e., UTF-16 code units) it takes to encode the character.</returns>
		public static int GetCharacterLength(StringBuilder stringBuilder, int index)
		{
			// check arguments
			if (stringBuilder == null)
				throw new ArgumentNullException("stringBuilder");
			if (index < 0 || index >= stringBuilder.Length)
				throw new ArgumentOutOfRangeException("index");

			return index < stringBuilder.Length - 1 && char.IsSurrogatePair(stringBuilder[index], stringBuilder[index + 1]) ? 2 : 1;
		}

		/// <summary>
		/// Converts the value of a UTF-16 encoded character or surrogate pair at a specified position in a <see cref="StringBuilder"/> into a Unicode code point.
		/// </summary>
		/// <param name="stringBuilder">A <see cref="StringBuilder"/> that contains a character or surrogate pair.</param>
		/// <param name="index">The index position of the character or surrogate pair in <paramref name="stringBuilder"/>.</param>
		/// <returns>The 21-bit Unicode code point represented by the character or surrogate pair at the position in the
		/// <paramref name="stringBuilder"/> parameter specified by the <paramref name="index"/> parameter.</returns>
		/// <remarks>This method behaves the same as <see cref="char.ConvertToUtf32(string,int)"/>, but operates on a <see cref="StringBuilder"/>
		/// instead of a <see cref="string"/>.</remarks>
		public static int GetCodePoint(StringBuilder stringBuilder, int index)
		{
			// check arguments
			if (stringBuilder == null)
				throw new ArgumentNullException("stringBuilder");
			if (index < 0 || index >= stringBuilder.Length)
				throw new ArgumentOutOfRangeException("index");

			// convert surrogate pair to supplementary codepoint, or simply return BMP codepoint
			char ch = stringBuilder[index];
			if (char.IsSurrogate(ch))
				return char.ConvertToUtf32(ch, index < stringBuilder.Length - 1 ? stringBuilder[index + 1] : '\0');
			else
				return ch;
		}
	}
}
