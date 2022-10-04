using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ComparisonUtilityTests
	{
		[TestCase(int.MinValue, 1)]
		[TestCase(0, 1)]
		[TestCase(1, 1)]
		[TestCase(2, 1)]
		[TestCase(int.MaxValue, 1)]
		public void MinAndMaxWithInt32s(int nFirst, int nSecond)
		{
			Assert.AreEqual(Math.Min(nFirst, nSecond), ComparisonUtility.Min(nFirst, nSecond));
			Assert.AreEqual(Math.Max(nFirst, nSecond), ComparisonUtility.Max(nFirst, nSecond));
		}

		[TestCase(null, null)]
		[TestCase(null, "1.0")]
		[TestCase("1.0", null)]
		[TestCase("0.9", "1.0")]
		[TestCase("1.0", "1.0")]
		[TestCase("1.1", "1.0")]
		public void MinAndMaxWithVersions(string strVersionFirst, string strVersionSecond)
		{
			var verFirst = strVersionFirst is null ? null : new Version(strVersionFirst);
			var verSecond = strVersionSecond is null ? null : new Version(strVersionSecond);

			var verExpectedMax = verFirst is null ? verSecond :
				verSecond is null ? verFirst :
				verFirst >= verSecond ? verFirst : verSecond;
			var verMax = ComparisonUtility.Max(verFirst, verSecond);
			Assert.AreEqual(verExpectedMax, verMax);

			var verExpectedMin = verFirst is null ? verFirst :
				verSecond is null ? verSecond :
				verFirst <= verSecond ? verFirst : verSecond;
			var verMin = ComparisonUtility.Min(verFirst, verSecond);
			Assert.AreEqual(verExpectedMin, verMin);

			if (verFirst?.Equals(verSecond) is true)
			{
				Assert.AreSame(verFirst, verMax);
				Assert.AreSame(verFirst, verMin);
			}
		}

		[TestCase(1, 2, 1)]
		[TestCase(2, 2, 0)]
		[TestCase(3, 2, -1)]
		public void CreateComparer(int nLeft, int nRight, int nResult)
		{
			var comparer = ComparisonUtility.CreateComparer<int>((x, y) => -x.CompareTo(y));
			Assert.AreEqual(nResult, NormalizeResult(comparer.Compare(nLeft, nRight)));
		}

		[TestCase(11, 12, -1)]
		[TestCase(12, 12, 0)]
		[TestCase(13, 12, 1)]
		[TestCase(11, 22, -1)]
		[TestCase(12, 22, -1)]
		[TestCase(13, 22, 1)]
		public void ChainedCompare(int nLeft, int nRight, int nResult)
		{
			var comparer = ComparisonUtility.CreateComparer<int>((x, y) => (x % 10).CompareTo(y % 10), (x, y) => (x / 10).CompareTo(y / 10));
			Assert.AreEqual(nResult, NormalizeResult(comparer.Compare(nLeft, nRight)));

			Assert.AreEqual(nResult, NormalizeResult(ComparisonUtility.ChainedCompare(nLeft, nRight, (x, y) => (x % 10).CompareTo(y % 10), (x, y) => (x / 10).CompareTo(y / 10))));

			var comparison = ComparisonUtility.CreateChainedComparison<int>((x, y) => (x % 10).CompareTo(y % 10), (x, y) => (x / 10).CompareTo(y / 10));
			Assert.AreEqual(nResult, NormalizeResult(comparison(nLeft, nRight)));
		}

		[Test]
		public void NoComparers()
		{
			Assert.Throws<ArgumentException>(() => ComparisonUtility.CreateComparer<int>());
		}

		private static int NormalizeResult(int n)
		{
			return n < 0 ? -1 : n > 0 ? 1 : 0;
		}
	}
}
