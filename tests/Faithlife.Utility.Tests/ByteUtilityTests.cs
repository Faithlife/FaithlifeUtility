using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ByteUtilityTests
	{
		[Test]
		public void ToBytesArguments()
		{
			Assert.Throws<ArgumentNullException>(() => ByteUtility.ToBytes(null!));
			Assert.Throws<ArgumentException>(() => ByteUtility.ToBytes("1"));
			Assert.Throws<ArgumentOutOfRangeException>(() => ByteUtility.ToBytes("XY"));
		}

		[Test]
		public void ToStringArguments()
		{
			Assert.Throws<ArgumentNullException>(() => ByteUtility.ToString(null!));
		}

		[TestCase("", new byte[0])]
		[TestCase("12", new byte[] { 0x12 })]
		[TestCase("abCDEf", new byte[] { 0xab, 0xcd, 0xef })]
		[TestCase("000102030405060708090A0B0C0D0E0F", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 })]
		[TestCase("00102030405060708090A0B0C0D0E0F0", new byte[] { 0, 16, 32, 48, 64, 80, 96, 112, 128, 144, 160, 176, 192, 208, 224, 240 })]
		public void RoundTrip(string input, byte[] expected)
		{
			byte[] actual = ByteUtility.ToBytes(input);
			CollectionAssert.AreEqual(expected, actual);

			string output = ByteUtility.ToString(actual);
			Assert.AreEqual(input.ToUpperInvariant(), output);
		}
	}
}
