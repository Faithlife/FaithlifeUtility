using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class EnumUtilityTests
	{
		[Test]
		public void Parse()
		{
			Assert.AreEqual(DayOfWeek.Thursday, EnumUtility.Parse<DayOfWeek>("Thursday"));
			Assert.Throws<ArgumentException>(() => EnumUtility.Parse<StringSplitOptions>("Thursday"));
			Assert.Throws<ArgumentException>(() => EnumUtility.Parse<StringSplitOptions>("removeemptyentries"));
			Assert.Throws<ArgumentException>(() => EnumUtility.Parse<StringSplitOptions>("removeemptyentries", CaseSensitivity.MatchCase));
			Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, EnumUtility.Parse<StringSplitOptions>("removeemptyentries", CaseSensitivity.IgnoreCase));
		}

		[Test]
		public void TryParse()
		{
			DayOfWeek dow;
			Assert.IsTrue(EnumUtility.TryParse("Thursday", out dow));
			Assert.AreEqual(DayOfWeek.Thursday, dow);
			StringSplitOptions sso;
			Assert.IsFalse(EnumUtility.TryParse("Thursday", out sso));
			Assert.IsFalse(EnumUtility.TryParse("removeemptyentries", out sso));
			Assert.IsFalse(EnumUtility.TryParse("removeemptyentries", CaseSensitivity.MatchCase, out sso));
			Assert.IsTrue(EnumUtility.TryParse("removeemptyentries", CaseSensitivity.IgnoreCase, out sso));
			Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, sso);
		}

		[Test]
		public void TryParseNullable()
		{
			Assert.AreEqual(DayOfWeek.Thursday, EnumUtility.TryParse<DayOfWeek>("Thursday"));
			Assert.IsNull(EnumUtility.TryParse<StringSplitOptions>("Thursday"));
			Assert.IsNull(EnumUtility.TryParse<StringSplitOptions>("removeemptyentries"));
			Assert.IsNull(EnumUtility.TryParse<StringSplitOptions>("removeemptyentries", CaseSensitivity.MatchCase));
			Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, EnumUtility.TryParse<StringSplitOptions>("removeemptyentries", CaseSensitivity.IgnoreCase));
		}

		[Test]
		public void GetValues()
		{
			CollectionAssert.AreEqual(
				new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday },
				EnumUtility.GetValues<DayOfWeek>());
			CollectionAssert.AreEqual(new[] { StringSplitOptions.None, StringSplitOptions.RemoveEmptyEntries }, EnumUtility.GetValues<StringSplitOptions>());
		}
	}
}
