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
		/// <param name="s">A <see cref="string"/>.</param>
		/// <param name="nIndex">The character position in <paramref name="s"/>.</param>
		/// <returns>A <see cref="UnicodeCharacterClass"/> enumerated constant that identifies the character class of the character at position <paramref name="nIndex"/> in <paramref name="s"/>.</returns>
		public static UnicodeCharacterClass GetCharacterClass(string s, int nIndex)
		{
			return GetCharacterClassFromCategory(CharUnicodeInfo.GetUnicodeCategory(s, nIndex));
		}

		/// <summary>
		/// Returns the major character class for the specified Unicode general category.
		/// </summary>
		/// <param name="cat">The <see cref="UnicodeCategory"/> of a character.</param>
		/// <returns>The class of the specified category.</returns>
		public static UnicodeCharacterClass GetCharacterClassFromCategory(UnicodeCategory cat)
		{
			if (cat == UnicodeCategory.UppercaseLetter || cat == UnicodeCategory.LowercaseLetter || cat == UnicodeCategory.TitlecaseLetter ||
				cat == UnicodeCategory.ModifierLetter || cat == UnicodeCategory.OtherLetter)
				return UnicodeCharacterClass.Letter;
			else if (cat == UnicodeCategory.NonSpacingMark || cat == UnicodeCategory.SpacingCombiningMark || cat == UnicodeCategory.EnclosingMark)
				return UnicodeCharacterClass.Mark;
			else if (cat == UnicodeCategory.DecimalDigitNumber || cat == UnicodeCategory.LetterNumber || cat == UnicodeCategory.OtherNumber)
				return UnicodeCharacterClass.Number;
			else if (cat == UnicodeCategory.SpaceSeparator || cat == UnicodeCategory.LineSeparator || cat == UnicodeCategory.ParagraphSeparator)
				return UnicodeCharacterClass.Separator;
			else if (cat == UnicodeCategory.ConnectorPunctuation || cat == UnicodeCategory.DashPunctuation || cat == UnicodeCategory.OpenPunctuation ||
				cat == UnicodeCategory.ClosePunctuation || cat == UnicodeCategory.InitialQuotePunctuation || cat == UnicodeCategory.FinalQuotePunctuation ||
				cat == UnicodeCategory.OtherPunctuation)
				return UnicodeCharacterClass.Punctuation;
			else if (cat == UnicodeCategory.MathSymbol || cat == UnicodeCategory.CurrencySymbol || cat == UnicodeCategory.ModifierSymbol ||
				cat == UnicodeCategory.OtherSymbol)
				return UnicodeCharacterClass.Symbol;
			else
				return UnicodeCharacterClass.Other;
		}

		/// <summary>
		/// Gets the length (in chars) of the Unicode character at the specified offset in the given string.
		/// </summary>
		/// <param name="s">A <see cref="string"/>.</param>
		/// <param name="nIndex">The character position in <paramref name="s"/>.</param>
		/// <returns>The number of chars (i.e., UTF-16 code units) it takes to encode the character.</returns>
		public static int GetCharacterLength(string s, int nIndex)
		{
			return char.IsSurrogatePair(s, nIndex) ? 2 : 1;
		}

		/// <summary>
		/// Gets the length (in chars) of the Unicode character at the specified offset in the given <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="sb">A <see cref="StringBuilder"/>.</param>
		/// <param name="nIndex">The character position in <paramref name="sb"/>.</param>
		/// <returns>The number of chars (i.e., UTF-16 code units) it takes to encode the character.</returns>
		public static int GetCharacterLength(StringBuilder sb, int nIndex)
		{
			// check arguments
			if (sb == null)
				throw new ArgumentNullException("sb");
			if (nIndex < 0 || nIndex >= sb.Length)
				throw new ArgumentOutOfRangeException("nIndex");

			return nIndex < sb.Length - 1 && char.IsSurrogatePair(sb[nIndex], sb[nIndex + 1]) ? 2 : 1;
		}

		/// <summary>
		/// Converts the value of a UTF-16 encoded character or surrogate pair at a specified position in a <see cref="StringBuilder"/> into a Unicode code point.
		/// </summary>
		/// <param name="sb">A <see cref="StringBuilder"/> that contains a character or surrogate pair.</param>
		/// <param name="index">The index position of the character or surrogate pair in <paramref name="sb"/>.</param>
		/// <returns>The 21-bit Unicode code point represented by the character or surrogate pair at the position in the
		/// <paramref name="sb"/> parameter specified by the <paramref name="index"/> parameter.</returns>
		/// <remarks>This method behaves the same as <see cref="char.ConvertToUtf32(string,int)"/>, but operates on a <see cref="StringBuilder"/>
		/// instead of a <see cref="string"/>.</remarks>
		public static int GetCodePoint(StringBuilder sb, int index)
		{
			// check arguments
			if (sb == null)
				throw new ArgumentNullException("sb");
			if (index < 0 || index >= sb.Length)
				throw new ArgumentOutOfRangeException("index");

			// convert surrogate pair to supplementary codepoint, or simply return BMP codepoint
			char ch = sb[index];
			if (char.IsSurrogate(ch))
				return char.ConvertToUtf32(ch, index < sb.Length - 1 ? sb[index + 1] : '\0');
			else
				return ch;
		}
	}
}
