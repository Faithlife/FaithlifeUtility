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
			var dtExpected = new DateTimeOffset(2006, 1, 2, 3, 4, 5, TimeSpan.FromHours(-8));
			var dtParsed = DateTimeOffsetUtility.ParseIso8601("2006-01-02T03:04:05-08:00");
			Assert.AreEqual(dtExpected, dtParsed);
		}

		[Test]
		public void BadInput()
		{
			Assert.Throws<ArgumentNullException>(() => DateTimeOffsetUtility.ParseIso8601(null!));
			Assert.Throws<FormatException>(() => DateTimeOffsetUtility.ParseIso8601(""));

			Assert.IsFalse(DateTimeOffsetUtility.TryParseIso8601(null, out var dt));
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
			var dtExpected = new DateTimeOffset(2006, 1, 2, 3, 4, 5, 0, TimeSpan.FromHours(-8));
			Assert.IsTrue(DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05-08:00", out var dtParsed));
			Assert.AreEqual(dtExpected, dtParsed);
			Assert.AreEqual(dtExpected, DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05-08:00"));
		}

		[Test]
		public void TryParseIso8601MissingTimezone()
		{
			Assert.IsFalse(DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05", out var dtParsed));
			Assert.IsNull(DateTimeOffsetUtility.TryParseIso8601("2006-01-02T03:04:05"));
		}

		[Test]
		public void ToIso8601()
		{
			var dt = new DateTimeOffset(2006, 1, 2, 3, 4, 5, 0, TimeSpan.FromHours(-8));
			Assert.AreEqual("2006-01-02T03:04:05-08:00", dt.ToIso8601());
		}

		[Test]
		public void RoundTripLocal()
		{
			var dtNow = ClearMilliseconds(DateTimeOffset.Now);
			var strRendered = dtNow.ToIso8601();
			var dtParsed = DateTimeOffsetUtility.ParseIso8601(strRendered);
			Assert.AreEqual(dtNow, dtParsed);
		}

		[Test]
		public void RoundTripUtc()
		{
			var dtNow = ClearMilliseconds(DateTimeOffset.UtcNow);
			var strRendered = dtNow.ToIso8601();
			var dtParsed = DateTimeOffsetUtility.ParseIso8601(strRendered);
			Assert.AreEqual(dtNow, dtParsed);
		}

		[Test]
		public void ParseLegacyFormat()
		{
			var strDateTime = "2009-06-09T14:59:23Z";
			var dtExpected = new DateTimeOffset(2009, 6, 9, 14, 59, 23, default);

			var dt = DateTimeOffsetUtility.ParseIso8601(strDateTime);
			Assert.AreEqual(dtExpected, dt);

			Assert.IsTrue(DateTimeOffsetUtility.TryParseIso8601(strDateTime, out var dt2));
			Assert.AreEqual(dtExpected, dt2);

			var strUtcNow = DateTime.UtcNow.ToIso8601();
			var dtUtcNow = DateTimeUtility.ParseIso8601(strUtcNow);
			var dtoUtcNow = DateTimeOffsetUtility.ParseIso8601(strUtcNow);
			Assert.AreEqual(dtoUtcNow.DateTime, dtUtcNow);
		}

		private DateTimeOffset ClearMilliseconds(DateTimeOffset dt)
		{
			return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Offset);
		}
	}
}
