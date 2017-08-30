using System;
using System.Globalization;
using System.Text;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class UnicodeUtilityTests
	{
		[TestCase(UnicodeCategory.UppercaseLetter, UnicodeCharacterClass.Letter)]
		[TestCase(UnicodeCategory.LowercaseLetter, UnicodeCharacterClass.Letter)]
		[TestCase(UnicodeCategory.TitlecaseLetter, UnicodeCharacterClass.Letter)]
		[TestCase(UnicodeCategory.ModifierLetter, UnicodeCharacterClass.Letter)]
		[TestCase(UnicodeCategory.OtherLetter, UnicodeCharacterClass.Letter)]
		[TestCase(UnicodeCategory.NonSpacingMark, UnicodeCharacterClass.Mark)]
		[TestCase(UnicodeCategory.SpacingCombiningMark, UnicodeCharacterClass.Mark)]
		[TestCase(UnicodeCategory.EnclosingMark, UnicodeCharacterClass.Mark)]
		[TestCase(UnicodeCategory.DecimalDigitNumber, UnicodeCharacterClass.Number)]
		[TestCase(UnicodeCategory.LetterNumber, UnicodeCharacterClass.Number)]
		[TestCase(UnicodeCategory.OtherNumber, UnicodeCharacterClass.Number)]
		[TestCase(UnicodeCategory.SpaceSeparator, UnicodeCharacterClass.Separator)]
		[TestCase(UnicodeCategory.LineSeparator, UnicodeCharacterClass.Separator)]
		[TestCase(UnicodeCategory.ParagraphSeparator, UnicodeCharacterClass.Separator)]
		[TestCase(UnicodeCategory.Control, UnicodeCharacterClass.Other)]
		[TestCase(UnicodeCategory.Format, UnicodeCharacterClass.Other)]
		[TestCase(UnicodeCategory.Surrogate, UnicodeCharacterClass.Other)]
		[TestCase(UnicodeCategory.PrivateUse, UnicodeCharacterClass.Other)]
		[TestCase(UnicodeCategory.ConnectorPunctuation, UnicodeCharacterClass.Punctuation)]
		[TestCase(UnicodeCategory.DashPunctuation, UnicodeCharacterClass.Punctuation)]
		[TestCase(UnicodeCategory.OpenPunctuation, UnicodeCharacterClass.Punctuation)]
		[TestCase(UnicodeCategory.ClosePunctuation, UnicodeCharacterClass.Punctuation)]
		[TestCase(UnicodeCategory.InitialQuotePunctuation, UnicodeCharacterClass.Punctuation)]
		[TestCase(UnicodeCategory.FinalQuotePunctuation, UnicodeCharacterClass.Punctuation)]
		[TestCase(UnicodeCategory.OtherPunctuation, UnicodeCharacterClass.Punctuation)]
		[TestCase(UnicodeCategory.MathSymbol, UnicodeCharacterClass.Symbol)]
		[TestCase(UnicodeCategory.CurrencySymbol, UnicodeCharacterClass.Symbol)]
		[TestCase(UnicodeCategory.ModifierSymbol, UnicodeCharacterClass.Symbol)]
		[TestCase(UnicodeCategory.OtherSymbol, UnicodeCharacterClass.Symbol)]
		[TestCase(UnicodeCategory.OtherNotAssigned, UnicodeCharacterClass.Other)]
		public void ClassFromCategory(UnicodeCategory cat, UnicodeCharacterClass classExpected)
		{
			Assert.AreEqual(classExpected, UnicodeUtility.GetCharacterClassFromCategory(cat));
		}

		[TestCase("a\uD800\uDF80!", 0, UnicodeCharacterClass.Letter)]
		[TestCase("a\uD800\uDF80!", 1, UnicodeCharacterClass.Letter)]
		[TestCase("a\uD800\uDF80!", 2, UnicodeCharacterClass.Other)]
		[TestCase("a\uD800\uDF80!", 3, UnicodeCharacterClass.Punctuation)]
		[TestCase("1!a $\uE000", 0, UnicodeCharacterClass.Number)]
		[TestCase("1!a $\uE000", 1, UnicodeCharacterClass.Punctuation)]
		[TestCase("1!a $\uE000", 2, UnicodeCharacterClass.Letter)]
		[TestCase("1!a $\uE000", 3, UnicodeCharacterClass.Separator)]
		[TestCase("1!a $\uE000", 4, UnicodeCharacterClass.Symbol)]
		[TestCase("1!a $\uE000", 5, UnicodeCharacterClass.Other)]
		public void CharacterClass(string s, int nIndex, UnicodeCharacterClass classExpected)
		{
			Assert.AreEqual(classExpected, UnicodeUtility.GetCharacterClass(s, nIndex));
		}

		[Test]
		public void CharacterClassNullString()
		{
			Assert.Throws<ArgumentNullException>(() => UnicodeUtility.GetCharacterClass(null, 1));
		}

		[Test]
		public void CharacterClassNegativeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCharacterClass("a", -1));
		}

		[Test]
		public void CharacterClassLargeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCharacterClass("a", 1));
		}

		[Test]
		public void CharacterLength()
		{
			string s1 = "1!a $\uE000";
			StringBuilder sb1 = new StringBuilder(s1);
			for (int nIndex = 0; nIndex < s1.Length; ++nIndex)
			{
				Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(s1, nIndex));
				Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(s1, nIndex));
				Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(sb1, nIndex));
				Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(sb1, nIndex));
			}

			string s2 = "\uDF80\uD800";
			StringBuilder sb2 = new StringBuilder(s2);
			for (int nIndex = 0; nIndex < s2.Length; ++nIndex)
			{
				Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(s2, nIndex));
				Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(sb2, nIndex));
			}

			string s3 = "\uD800\uDF80";
			StringBuilder sb3 = new StringBuilder(s3);
			Assert.AreEqual(2, UnicodeUtility.GetCharacterLength(s3, 0));
			Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(s3, 1));
			Assert.AreEqual(2, UnicodeUtility.GetCharacterLength(sb3, 0));
			Assert.AreEqual(1, UnicodeUtility.GetCharacterLength(sb3, 1));
		}

		[Test]
		public void CharacterLengthNullString()
		{
			Assert.Throws<ArgumentNullException>(() => UnicodeUtility.GetCharacterLength((string) null, 1));
			Assert.Throws<ArgumentNullException>(() => UnicodeUtility.GetCharacterLength((StringBuilder) null, 1));
		}

		[Test]
		public void CharacterLengthNegativeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCharacterLength("a", -1));
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCharacterLength(new StringBuilder("a"), -1));
		}

		[Test]
		public void CharacterLengthLargeIndex()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCharacterLength("a", 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCharacterLength(new StringBuilder("a"), 1));
		}

		[TestCase("a", 0, 0x61)]
		[TestCase("\U00010380", 0, 0x10380)]
		[TestCase("abc", 1, 0x62)]
		[TestCase("a\U0010FFFFc", 1, 0x10FFFF)]
		public void GetCodePoint(string s, int index, int expected)
		{
			StringBuilder sb = new StringBuilder(s);
			Assert.AreEqual(expected, UnicodeUtility.GetCodePoint(sb, index));
			Assert.AreEqual(char.ConvertToUtf32(s, index), UnicodeUtility.GetCodePoint(sb, index));
		}

		[Test]
		public void GetCodePointArgumentException()
		{
			Assert.Throws<ArgumentNullException>(() => UnicodeUtility.GetCodePoint(null, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCodePoint(new StringBuilder("a"), -1));
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCodePoint(new StringBuilder("a"), 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeUtility.GetCodePoint(new StringBuilder("\uD800"), 0));
		}
	}
}
