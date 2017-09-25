using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class DateTimeOffsetUtilityTests
	{
		[Test]
		public void ParseIso8601Good()
		{
			DateTimeOffset dtExpected = new DateTimeOffset(2006, 1, 2, 3, 4, 5, TimeSpan.FromHours(-8));
			DateTimeOffset dtParsed = DateTimeOffsetUtility.ParseIso8601("2006-01-02T03:04:05-08:00");
			Assert.AreEqual(dtExpected, dtParsed);
		}

		[Test]
		public void BadInput()
		{
			Assert.Throws<ArgumentNullException>(() => DateTimeOffsetUtility.ParseIso8601(null));
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601(""));

			DateTimeOffset dt;
			Assert.IsFalse(DateTimeOffsetUtility.TryParseIso8601(null, out dt));
			Assert.IsFalse(DateTimeOffsetUtility.TryParseIso8601("", out dt));
			Assert.IsNull(DateTimeOffsetUtility.TryParseIso8601(null));
			Assert.IsNull(DateTimeOffsetUtility.TryParseIso8601(""));
		}

		[Test]
		public void ParseIso8601MissingTimezone()
		{
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601("2006-01-02T03:04:05"));
		}

		[Test]
		public void ParseIso8601BadMonth()
		{
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601("2006-13-02T03:04:05"));
		}

		[Test]
		public void ParseIso8601BadDay()
		{
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601("2006-02-30T03:04:05"));
		}

		[Test]
		public void ParseIso8601BadHour()
		{
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601("2006-01-02T25:04:05"));
		}

		[Test]
		public void ParseIso8601BadMinute()
		{
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601("2006-01-02T03:61:05"));
		}

		[Test]
		public void ParseIso8601BadSecond()
		{
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601("2006-01-02T03:04:62"));
		}

		[Test]
		public void TryParseIso8601Good()
		{
			DateTimeOffset dtExpected = new DateTimeOffset(2006, 1, 2, 3, 4, 5, 0, TimeSpan.FromHours(-8));
			DateTimeOffset dtParsed;
			Assert.IsTrue(DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05-08:00", out dtParsed));
			Assert.AreEqual(dtExpected, dtParsed);
			Assert.AreEqual(dtExpected, DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05-08:00"));
		}

		[Test]
		public void TryParseIso8601MissingTimezone()
		{
			DateTimeOffset dtParsed;
			Assert.IsFalse(DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05", out dtParsed));
			Assert.IsNull(DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05"));
		}

		[Test]
		public void ToIso8601()
		{
			DateTimeOffset dt = new DateTimeOffset(2006, 1, 2, 3, 4, 5, 0, TimeSpan.FromHours(-8));
			Assert.AreEqual("2006-01-02T03:04:05-08:00", dt.ToIso8601());
		}

		[Test]
		public void RoundTripLocal()
		{
			DateTimeOffset dtNow = ClearMilliseconds(DateTimeOffset.Now);
			string strRendered = dtNow.ToIso8601();
			DateTimeOffset dtParsed = DateTimeOffsetUtility.ParseIso8601(strRendered);
			Assert.AreEqual(dtNow, dtParsed);
		}

		[Test]
		public void RoundTripUtc()
		{
			DateTimeOffset dtNow = ClearMilliseconds(DateTimeOffset.UtcNow);
			string strRendered = dtNow.ToIso8601();
			DateTimeOffset dtParsed = DateTimeOffsetUtility.ParseIso8601(strRendered);
			Assert.AreEqual(dtNow, dtParsed);
		}

		[Test]
		public void ParseLegacyFormat()
		{
			string strDateTime = "2009-06-09T14:59:23Z";
			DateTimeOffset dtExpected = new DateTimeOffset(2009, 6, 9, 14, 59, 23, new TimeSpan());

			DateTimeOffset dt = DateTimeOffsetUtility.ParseIso8601(strDateTime);
			Assert.AreEqual(dtExpected, dt);

			DateTimeOffset dt2;
			Assert.IsTrue(DateTimeOffsetUtility.TryParseIso8601(strDateTime, out dt2));
			Assert.AreEqual(dtExpected, dt2);

			string strUtcNow = DateTime.UtcNow.ToIso8601();
			DateTime dtUtcNow = DateTimeUtility.ParseIso8601(strUtcNow);
			DateTimeOffset dtoUtcNow = DateTimeOffsetUtility.ParseIso8601(strUtcNow);
			Assert.AreEqual(dtoUtcNow.DateTime, dtUtcNow);
		}

		private DateTimeOffset ClearMilliseconds(DateTimeOffset dt)
		{
			return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Offset);
		}
	}
}
