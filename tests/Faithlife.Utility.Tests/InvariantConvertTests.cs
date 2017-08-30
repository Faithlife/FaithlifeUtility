using System;
using Faithlife.Utility.Invariant;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class InvariantConvertTests
	{
		[TestCase(null, null, null)]
		[TestCase("", null, null)]
		[TestCase(" ", null, null)]
		[TestCase("True", true, "true")]
		[TestCase("true", true, "true")]
		[TestCase("tRuE", true, "true")]
		[TestCase(" True", true, "true")]
		[TestCase("true ", true, "true")]
		[TestCase("False", false, "false")]
		[TestCase("false", false, "false")]
		[TestCase("fAlSe", false, "false")]
		[TestCase(" False", false, "false")]
		[TestCase("false ", false, "false")]
		[TestCase("0", null, null)]
		public void TestBoolean(string before, object value, string after)
		{
			Assert.AreEqual((bool?) value, InvariantConvert.TryParseBoolean(before));

			try
			{
				bool parsed = InvariantConvert.ParseBoolean(before);
				Assert.AreEqual((bool) value, parsed);
			}
			catch (FormatException)
			{
				Assert.IsNull(value);
			}

			if (after != null)
				Assert.AreEqual(after, ((bool) value).ToInvariantString());
		}

		[TestCase("3", 3.0, "3")]
		[TestCase("0", 0.0, "0")]
		[TestCase("-3", -3.0, "-3")]
		[TestCase("4.94065645841247E-324", double.Epsilon, "4.94065645841247E-324")]
		[TestCase("1.7976931348623157E+308", double.MaxValue, "1.7976931348623157E+308")]
		[TestCase("-1.7976931348623157E+308", double.MinValue, "-1.7976931348623157E+308")]
		[TestCase("+0006.0221412927000e23", 6.0221412927E+23, "6.0221412927E+23")]
		[TestCase("12345678901234567890", 1.2345678901234567E+19, "1.2345678901234567E+19")]
		[TestCase("0.12345678901234567890", 0.12345678901234568, "0.12345678901234568")]
		[TestCase("NaN", double.NaN, "NaN")]
		[TestCase("-Infinity", double.NegativeInfinity, "-Infinity")]
		[TestCase("Infinity", double.PositiveInfinity, "Infinity")]
		[TestCase(" NaN ", double.NaN, "NaN")]
		[TestCase(" -Infinity ", double.NegativeInfinity, "-Infinity")]
		[TestCase(" Infinity ", double.PositiveInfinity, "Infinity")]
		[TestCase("0.33333333333333331", 1.0 / 3.0, "0.33333333333333331")]
		[TestCase(null, null, null)]
		[TestCase("", null, null)]
		[TestCase(" ", null, null)]
		[TestCase(" -3 ", -3.0, "-3")]
		[TestCase("3 4", null, null)]
		[TestCase("3.", 3.0, "3")]
		[TestCase("-0", 0.0, "0")]
		[TestCase("-0", -0.0, "-0")]
		[TestCase("-infinity", null, null)]
		[TestCase("infinity", null, null)]
		[TestCase("nan", null, null)]
		public void TestDouble(string before, object value, string after)
		{
			Assert.AreEqual((double?) value, InvariantConvert.TryParseDouble(before));

			try
			{
				double parsed = InvariantConvert.ParseDouble(before);
				Assert.AreEqual((double) value, parsed);
			}
			catch (FormatException)
			{
				Assert.IsNull(value);
			}

			if (after != null)
				Assert.AreEqual(after, ((double) value).ToInvariantString());
		}

		[TestCase("3", 3, "3")]
		[TestCase("0", 0, "0")]
		[TestCase("-3", -3, "-3")]
		[TestCase("2147483647", int.MaxValue, "2147483647")]
		[TestCase("-2147483648", int.MinValue, "-2147483648")]
		[TestCase("03", 3, "3")]
		[TestCase("-0", 0, "0")]
		[TestCase(null, null, null)]
		[TestCase("", null, null)]
		[TestCase(" ", null, null)]
		[TestCase(" -3 ", -3, "-3")]
		[TestCase("3 4", null, null)]
		[TestCase("3.", null, null)]
		[TestCase("+3", 3, "3")]
		[TestCase("2147483648", null, null)]
		[TestCase("-2147483649", null, null)]
		public void TestInt32(string before, object value, string after)
		{
			Assert.AreEqual((int?) value, InvariantConvert.TryParseInt32(before));

			try
			{
				int parsed = InvariantConvert.ParseInt32(before);
				Assert.AreEqual((int) value, parsed);
			}
			catch (FormatException)
			{
				Assert.IsNull(value);
			}

			if (after != null)
				Assert.AreEqual(after, ((int) value).ToInvariantString());
		}

		[TestCase("3", 3L, "3")]
		[TestCase("0", 0L, "0")]
		[TestCase("-3", -3L, "-3")]
		[TestCase("9223372036854775807", long.MaxValue, "9223372036854775807")]
		[TestCase("-9223372036854775808", long.MinValue, "-9223372036854775808")]
		[TestCase("03", 3L, "3")]
		[TestCase("-0", 0L, "0")]
		[TestCase(null, null, null)]
		[TestCase("", null, null)]
		[TestCase(" ", null, null)]
		[TestCase(" -3 ", -3L, "-3")]
		[TestCase("3 4", null, null)]
		[TestCase("3.", null, null)]
		[TestCase("+3", 3L, "3")]
		[TestCase("9223372036854775808", null, null)]
		[TestCase("-9223372036854775809", null, null)]
		public void TestInt64(string before, object value, string after)
		{
			Assert.AreEqual((long?) value, InvariantConvert.TryParseInt64(before));

			try
			{
				long parsed = InvariantConvert.ParseInt64(before);
				Assert.AreEqual((long) value, parsed);
			}
			catch (FormatException)
			{
				Assert.IsNull(value);
			}

			if (after != null)
				Assert.AreEqual(after, ((long) value).ToInvariantString());
		}

		[TestCase(null, null)]
		[TestCase("", null)]
		[TestCase("0:0", "00:00:00")]
		[TestCase("-0", "00:00:00")]
		[TestCase("23:59", "23:59:00")]
		[TestCase("-23:59", "-23:59:00")]
		[TestCase("23:60", null)]
		[TestCase("24:00", null)]
		[TestCase("1", "1.00:00:00")]
		[TestCase("1.0", null)]
		[TestCase(" 23:59", "23:59:00")]
		[TestCase("23:59 ", "23:59:00")]
		[TestCase("23 : 59", null)]
		[TestCase("23:59:59.0", "23:59:59")]
		[TestCase("23:59:59.9", "23:59:59.9000000")]
		[TestCase("23:59:59.9000000", "23:59:59.9000000")]
		[TestCase("23:59:59.90000000", null)]
		[TestCase("23:59:59.9999999", "23:59:59.9999999")]
		[TestCase("23:59:59.99999990", null)]
		[TestCase("10675199.02:48:05.4775807", "10675199.02:48:05.4775807")]
		[TestCase("10675199.02:48:05.4775808", null)]
		[TestCase("-10675199.02:48:05.4775808", "-10675199.02:48:05.4775808")]
		[TestCase("-10675199.02:48:05.4775809", null)]
		public void TestTimeSpan(string before, string after)
		{
			TimeSpan? value = after != null ? TimeSpan.Parse(after) : default(TimeSpan?);

			Assert.AreEqual(value, InvariantConvert.TryParseTimeSpan(before));

			try
			{
				TimeSpan parsed = InvariantConvert.ParseTimeSpan(before);
				Assert.AreEqual(value.Value, parsed);
			}
			catch (FormatException)
			{
				Assert.IsNull(value);
			}

			if (after != null)
				Assert.AreEqual(after, value.Value.ToInvariantString());
		}
	}
}
