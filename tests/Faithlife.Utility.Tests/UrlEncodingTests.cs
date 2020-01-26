using System;
using System.Text;
using System.Web;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class UrlEncodingTests
	{
		[TestCase("Hi%2c%20there", "Hi, there")]
		[TestCase("%e8%a5%90", "\u8950")]
		[TestCase(null, null)]
		[TestCase("NoEncoding", "NoEncoding")]
		public void EncodeString(string? strEncoded, string? strDecoded)
		{
			Assert.AreEqual(strEncoded, UrlEncoding.Encode(strDecoded));
		}

		[TestCase("Hi,%20there", "Hi, there")]
		[TestCase("Hi%2c there", "Hi, there")]
		[TestCase("Hi%2c%20there", "Hi, there")]
		[TestCase("Hi%2C%20there", "Hi, there")]
		[TestCase("%e8%a5%90", "\u8950")]
		[TestCase("%E8%A5%90", "\u8950")]
		[TestCase(null, null)]
		[TestCase("NoEncoding", "NoEncoding")]
		public void DecodeString(string? strEncoded, string? strDecoded)
		{
			Assert.AreEqual(strDecoded, UrlEncoding.Decode(strEncoded));
		}

		[TestCase("Hi,+there", "Hi, there")]
		[TestCase("Hi,+there%20Ed.", "Hi, there Ed.")]
		[TestCase(null, null)]
		[TestCase("NoEncoding", "NoEncoding")]
		public void DecodeStringWithSpaceSetting(string? strEncoded, string? strDecoded)
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.EncodedSpaceChar = '+';
			Assert.AreEqual(strDecoded, UrlEncoding.Decode(strEncoded, settings));
		}

		[Test]
		public void DecodingError()
		{
			Assert.AreEqual("Hi\uFFFD there", UrlEncoding.Decode("Hi%A1 there"));
		}

		[TestCase("Hi%2c+there", "Hi, there")]
		[TestCase("Hi%2c+there+Ed%2e", "Hi, there Ed.")]
		[TestCase(null, null)]
		[TestCase("NoEncoding", "NoEncoding")]
		public void EncodeStringWithSpaceSetting(string? strEncoded, string? strDecoded)
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.EncodedSpaceChar = '+';
			Assert.AreEqual(strEncoded, UrlEncoding.Encode(strDecoded, settings));
		}

		[TestCase("Hi,+th%65r%65", "Hi, there")]
		[TestCase("Plus:+%2b", "Plus: +")]
		[TestCase(null, null)]
		[TestCase("NoEncoding", "NoEncoding")]
		[TestCase("%2b+=+%25", "+ = %")]
		public void EncodeStringWithSpaceCustomShouldEncode(string? strEncoded, string? strDecoded)
		{
			UrlEncodingSettings settings = new UrlEncodingSettings
			{
				EncodedSpaceChar = '+',
				ShouldEncodeChar = ch => ch == 'e',
			};
			Assert.AreEqual(strEncoded, UrlEncoding.Encode(strDecoded, settings));
			Assert.AreEqual(strDecoded, UrlEncoding.Decode(strEncoded, settings));
		}

		[Test]
		public void EncodeNullSettings()
		{
			Assert.Throws<ArgumentNullException>(() => UrlEncoding.Encode("test", null!));
		}

		[Test]
		public void DecodeNullSettings()
		{
			Assert.Throws<ArgumentNullException>(() => UrlEncoding.Decode("test", null!));
		}

		[Test]
		public void ChangeBytePrefixCharEncode()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.EncodedBytePrefixChar = '+';
			Assert.AreEqual("Hi+2c+20there", UrlEncoding.Encode("Hi, there", settings));
			Assert.AreEqual("Hi+2c+20there+20Ed+2e", UrlEncoding.Encode("Hi, there Ed.", settings));
		}

		[Test]
		public void ChangeBytePrefixCharToBackslashEncode()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.EncodedBytePrefixChar = '\\';
			Assert.AreEqual("Hi\\2c\\20there", UrlEncoding.Encode("Hi, there", settings));
			Assert.AreEqual("Hi\\2c\\20there\\20Ed\\2e", UrlEncoding.Encode("Hi, there Ed.", settings));
		}

		[Test]
		public void ChangeBytePrefixCharDecode()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.EncodedBytePrefixChar = '+';
			Assert.AreEqual("Hi, there", UrlEncoding.Decode("Hi+2c+20there", settings));
			Assert.AreEqual("Hi, there Ed.", UrlEncoding.Decode("Hi+2c+20there+20Ed+2e", settings));
		}

		[Test]
		public void OnlyEncodeSpaces()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.ShouldEncodeChar = ch => ch == ' ';
			Assert.AreEqual("Hi,%20there", UrlEncoding.Encode("Hi, there", settings));
			Assert.AreEqual("Hi,%20there%20Ed.", UrlEncoding.Encode("Hi, there Ed.", settings));
		}

		[Test]
		public void ChangeEncoding()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.TextEncoding = Encoding.UTF32;
			Assert.AreEqual("Hi%2c%00%00%00%20%00%00%00there", UrlEncoding.Encode("Hi, there", settings));
			Assert.AreEqual("Hi%2c%00%00%00%20%00%00%00there%20%00%00%00Ed%2e%00%00%00", UrlEncoding.Encode("Hi, there Ed.", settings));
		}

		[Test]
		public void UppercaseHexDigits()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			Assert.AreEqual("Hi, there", UrlEncoding.Decode("Hi%2c%20there", settings));
			settings.UppercaseHexDigits = true;
			Assert.AreEqual("Hi, there", UrlEncoding.Decode("Hi%2C%20there", settings));
		}

		[Test]
		public void SettingsFromSettings()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings();
			settings.TextEncoding = Encoding.UTF32;

			UrlEncodingSettings settingsNew = settings.Clone();
			settingsNew.EncodedBytePrefixChar = '+';

			Assert.AreEqual("Hi+2c+00+00+00+20+00+00+00there", UrlEncoding.Encode("Hi, there", settingsNew));
			Assert.AreEqual("Hi+2c+00+00+00+20+00+00+00there+20+00+00+00Ed+2e+00+00+00", UrlEncoding.Encode("Hi, there Ed.", settingsNew));
		}

		// TODO: fix reference to HttpUtility
		//[Test]
		//public void HttpUtilityUrlEncode()
		//{
		//	UrlEncodingSettings settings = UrlEncodingSettings.HttpUtilitySettings;
		//	for (int x = 0x20; x <= 0x4FF; x++)
		//	{
		//		string str = char.ConvertFromUtf32(x);
		//		Assert.AreEqual(HttpUtility.UrlEncode(str), UrlEncoding.Encode(str, settings));
		//	}
		//}

		[Test]
		public void SettingsClone()
		{
			UrlEncodingSettings settings = new UrlEncodingSettings
			{
				EncodedBytePrefixChar = '!',
				EncodedSpaceChar = '$',
				PreventDoubleEncoding = true,
				UppercaseHexDigits = true,
				ShouldEncodeChar = ch => ch == '@',
				TextEncoding = Encoding.UTF32
			};
			UrlEncodingSettings settingsClone = settings.Clone();

			Assert.AreEqual(settings.EncodedBytePrefixChar, settingsClone.EncodedBytePrefixChar);
			Assert.AreEqual(settings.EncodedSpaceChar, settingsClone.EncodedSpaceChar);
			Assert.AreEqual(settings.PreventDoubleEncoding, settingsClone.PreventDoubleEncoding);
			Assert.AreEqual(settings.UppercaseHexDigits, settingsClone.UppercaseHexDigits);
			Assert.AreEqual(UrlEncoding.Encode("!@#$ 駉", settings), UrlEncoding.Encode("!@#$ 駉", settingsClone));
		}
	}
}
