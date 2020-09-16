using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class DateTimeUtilityTests
	{
		[Test]
		public void ParseIso8601Good()
		{
			var dtExpected = new DateTime(2006, 01, 02, 03, 04, 05, 0, DateTimeKind.Utc);
			var dtParsed = DateTimeUtility.ParseIso8601("2006-01-02T03:04:05Z");
			Assert.AreEqual(dtExpected, dtParsed);
		}

		[Test]
		public void ParseIso8601MissingTimezone()
		{
			Assert.Throws<FormatException>(() => DateTimeUtility.ParseIso8601("2006-01-02T03:04:05"));
		}

		[Test]
		public void ParseIso8601BadMonth()
		{
			Assert.Throws<FormatException>(() => DateTimeUtility.ParseIso8601("2006-13-02T03:04:05"));
		}

		[Test]
		public void ParseIso8601BadDay()
		{
			Assert.Throws<FormatException>(() => DateTimeUtility.ParseIso8601("2006-02-30T03:04:05"));
		}

		[Test]
		public void ParseIso8601BadHour()
		{
			Assert.Throws<FormatException>(() => DateTimeUtility.ParseIso8601("2006-01-02T25:04:05"));
		}

		[Test]
		public void ParseIso8601BadMinute()
		{
			Assert.Throws<FormatException>(() => DateTimeUtility.ParseIso8601("2006-01-02T03:61:05"));
		}

		[Test]
		public void ParseIso8601BadSecond()
		{
			Assert.Throws<FormatException>(() => DateTimeUtility.ParseIso8601("2006-01-02T03:04:62"));
		}

		[Test]
		public void TryParseIso8601Good()
		{
			var dtExpected = new DateTime(2006, 01, 02, 03, 04, 05, 0, DateTimeKind.Utc);
			Assert.IsTrue(DateTimeUtility.TryParseIso8601("2006-01-02T03:04:05Z", out var dtParsed));
			Assert.AreEqual(dtExpected, dtParsed);

			var dtParsed2 = DateTimeUtility.TryParseIso8601("2006-01-02T03:04:05Z");
			Assert.IsTrue(dtParsed2.HasValue);
			Assert.AreEqual(dtExpected, dtParsed2!.Value);
		}

		[Test]
		public void TryParseIso8601MissingTimezone()
		{
			Assert.IsFalse(DateTimeUtility.TryParseIso8601("2006-01-02T03:04:05", out var dtParsed));
			Assert.IsFalse(DateTimeUtility.TryParseIso8601("2006-01-02T03:04:05").HasValue);
		}

		[Test]
		public void ToIso8601()
		{
			var dt = new DateTime(2006, 01, 02, 03, 04, 05, 0, DateTimeKind.Utc);
			Assert.AreEqual("2006-01-02T03:04:05Z", dt.ToIso8601());
		}

		[Test]
		public void RoundTripLocal()
		{
			var dtNow = ClearMilliseconds(DateTime.Now);
			var strRendered = dtNow.ToIso8601();
			var dtParsed = DateTimeUtility.ParseIso8601(strRendered);
			Assert.AreEqual(dtNow, dtParsed.ToLocalTime());
		}

		[Test]
		public void RoundTripUtc()
		{
			var dtNow = ClearMilliseconds(DateTime.UtcNow);
			var strRendered = dtNow.ToIso8601();
			var dtParsed = DateTimeUtility.ParseIso8601(strRendered);
			Assert.AreEqual(dtNow, dtParsed);
		}

		[Test]
		public void RoundTripUnixTimestamp()
		{
			var dtNow = ClearMilliseconds(DateTime.UtcNow);
			var unixTimestamp = DateTimeUtility.ToUnixTimestamp(dtNow);
			var dtFromUnixTimestamp = DateTimeUtility.FromUnixTimestamp(unixTimestamp);
			Assert.AreEqual(dtNow, dtFromUnixTimestamp);
		}

		[Test]
		public void AreSameSecondClearsMilliseconds()
		{
			var dt1 = new DateTime(2006, 1, 2, 3, 4, 5, 1, DateTimeKind.Utc);
			var dt2 = new DateTime(2006, 1, 2, 3, 4, 5, 999, DateTimeKind.Utc);
			Assert.IsTrue(DateTimeUtility.AreSameSecond(dt1, dt2));
		}

		[Test]
		public void AreSameSecondDoesNotRoundUp()
		{
			var dt1 = new DateTime(2006, 1, 2, 3, 4, 5, 999, DateTimeKind.Utc);
			var dt2 = new DateTime(2006, 1, 2, 3, 4, 6, 0, DateTimeKind.Utc);
			Assert.IsTrue(!DateTimeUtility.AreSameSecond(dt1, dt2));
		}

		[Test]
		public void SpecifyUtc()
		{
			DateTime? now = DateTime.Now;
			Assert.AreEqual(now.Value.Kind, DateTimeKind.Local);
			Assert.AreEqual(now.Value.SpecifyUtc().Kind, DateTimeKind.Utc);
			Assert.AreEqual(now.SpecifyUtc()!.Value.Kind, DateTimeKind.Utc);
			now = null;
			Assert.AreEqual(now.SpecifyUtc(), null);
		}

		private DateTime ClearMilliseconds(DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Kind);
		}
	}
}
