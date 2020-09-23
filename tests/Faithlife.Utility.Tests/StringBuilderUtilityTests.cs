using System.Text;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StringBuilderUtilityTests
	{
		[TestCase("False, True", "{0}, {1}", new object[] { false, true })]
		[TestCase("0, 255", "{0}, {1}", new object[] { 0, 255 })]
		[TestCase("-32768, 32767", "{0}, {1}", new object[] { short.MinValue, short.MaxValue })]
		public void AppendFormatInvariant(string strExpected, string strFormat, object[] values)
		{
			Assert.AreEqual(strExpected, new StringBuilder().AppendFormatInvariant(strFormat, values).ToString());
		}

		[TestCase(false, "False")]
		[TestCase(true, "True")]
		public void AppendInvariantBoolean(bool value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(byte.MinValue, "0")]
		[TestCase(byte.MaxValue, "255")]
		[TestCase(0, "0")]
		[TestCase(1, "1")]
		public void AppendInvariantByte(byte value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(short.MinValue, "-32768")]
		[TestCase(short.MaxValue, "32767")]
		[TestCase(-1, "-1")]
		[TestCase(0, "0")]
		[TestCase(1, "1")]
		public void AppendInvariantInt16(short value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(int.MinValue, "-2147483648")]
		[TestCase(int.MaxValue, "2147483647")]
		[TestCase(-1, "-1")]
		[TestCase(0, "0")]
		[TestCase(1, "1")]
		public void AppendInvariantInt32(int value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(long.MinValue, "-9223372036854775808")]
		[TestCase(long.MaxValue, "9223372036854775807")]
		[TestCase(-1, "-1")]
		[TestCase(0L, "0")]
		[TestCase(1L, "1")]
		public void AppendInvariantInt64(long value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(float.Epsilon, "1E-45")]
		[TestCase(1.5f, "1.5")]
		[TestCase(-1, "-1")]
		[TestCase(0f, "0")]
		[TestCase(1f, "1")]
		public void AppendInvariantSingle(float value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(double.Epsilon, "5E-324")]
		[TestCase(1.5, "1.5")]
		[TestCase(-1, "-1")]
		[TestCase(0, "0")]
		[TestCase(1, "1")]
		public void AppendInvariantDouble(double value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(1.5, "1.5")]
		[TestCase(-1, "-1")]
		[TestCase(0, "0")]
		[TestCase(1, "1")]
		public void AppendInvariantDecimal(decimal value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[Test]
		public void AppendInvariantDecimalMinMax()
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, decimal.MaxValue);
			Assert.AreEqual("79228162514264337593543950335", sb.ToString());

			sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, decimal.MinValue);
			Assert.AreEqual("-79228162514264337593543950335", sb.ToString());
		}

		[TestCase(sbyte.MinValue, "-128")]
		[TestCase(sbyte.MaxValue, "127")]
		[TestCase(-1, "-1")]
		[TestCase(0, "0")]
		[TestCase(1, "1")]
		public void AppendInvariantSByte(sbyte value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(ushort.MinValue, "0")]
		[TestCase(ushort.MaxValue, "65535")]
		[TestCase((ushort) 1, "1")]
		public void AppendInvariantUInt16(ushort value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(uint.MinValue, "0")]
		[TestCase(uint.MaxValue, "4294967295")]
		[TestCase(1u, "1")]
		public void AppendInvariantUInt32(uint value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}

		[TestCase(ulong.MinValue, "0")]
		[TestCase(ulong.MaxValue, "18446744073709551615")]
		[TestCase(1ul, "1")]
		public void AppendInvariantUInt64(ulong value, string strExpected)
		{
			var sb = new StringBuilder();
			StringBuilderUtility.AppendInvariant(sb, value);
			Assert.AreEqual(strExpected, sb.ToString());
		}
	}
}
