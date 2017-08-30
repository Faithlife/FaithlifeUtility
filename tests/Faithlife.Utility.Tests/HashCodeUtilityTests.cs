using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class HashCodeUtilityTests
	{
		[TestCase(0, 1800329511)]
		[TestCase(1, -1266253386)]
		[TestCase(2, -496519092)]
		[TestCase(3, 1332612764)]
		[TestCase(4, 946348061)]
		[TestCase(-1, -26951294)]
		[TestCase(-2, 2115881666)]
		public void GetPersistentIntHashCode(int nValue, int nExpected)
		{
			Assert.AreEqual(nExpected, HashCodeUtility.GetPersistentHashCode((sbyte) nValue));
			Assert.AreEqual(nExpected, HashCodeUtility.GetPersistentHashCode((short) nValue));
			if (nValue >= 0)
				Assert.AreEqual(nExpected, HashCodeUtility.GetPersistentHashCode((char) nValue));
			Assert.AreEqual(nExpected, HashCodeUtility.GetPersistentHashCode(nValue));
		}

		[TestCase(0, 720020139)]
		[TestCase(1, 357654460)]
		[TestCase(2, 715307540)]
		[TestCase(3, 1072960876)]
		[TestCase(4, 1430614333)]
		[TestCase(-1, 532412650)]
		[TestCase(-2, 340268856)]
		public void GetPersistentLongHashCode(long nValue, int nExpected)
		{
			Assert.AreEqual(nExpected, HashCodeUtility.GetPersistentHashCode(nValue));
		}

		[TestCase(false, 1800329511)]
		[TestCase(true, -1266253386)]
		public void GetPersistentBoolHashCode(bool value, int expected)
		{
			Assert.AreEqual(expected, HashCodeUtility.GetPersistentHashCode(value));
		}

		[Test]
		public void CombineHashCodes()
		{
			Assert.AreEqual(76781240, HashCodeUtility.CombineHashCodes(0));
			Assert.AreEqual(1923623579, HashCodeUtility.CombineHashCodes(1));
			Assert.AreEqual(76781240, HashCodeUtility.CombineHashCodes(new[] { 0 }));

			Assert.AreEqual(1489077439, HashCodeUtility.CombineHashCodes(0, 0));
			Assert.AreEqual(-1105120207, HashCodeUtility.CombineHashCodes(0, 1));
			Assert.AreEqual(1489077439, HashCodeUtility.CombineHashCodes(new[] { 0, 0 }));

			Assert.AreEqual(459859287, HashCodeUtility.CombineHashCodes(0, 0, 0));
			Assert.AreEqual(-935278042, HashCodeUtility.CombineHashCodes(0, 0, 1));
			Assert.AreEqual(459859287, HashCodeUtility.CombineHashCodes(new[] { 0, 0, 0 }));

			Assert.AreEqual(-1158084724, HashCodeUtility.CombineHashCodes(0, 0, 0, 0));
			Assert.AreEqual(1498382024, HashCodeUtility.CombineHashCodes(0, 0, 0, 1));
			Assert.AreEqual(-1158084724, HashCodeUtility.CombineHashCodes(new[] { 0, 0, 0, 0 }));

			Assert.AreEqual(1762635120, HashCodeUtility.CombineHashCodes(0, 0, 0, 0, 0));
			Assert.AreEqual(951675744, HashCodeUtility.CombineHashCodes(0, 0, 0, 0, 1));
			Assert.AreEqual(1762635120, HashCodeUtility.CombineHashCodes(new[] { 0, 0, 0, 0, 0 }));

			Assert.AreEqual(1002546726, HashCodeUtility.CombineHashCodes(0, 0, 0, 0, 0, 0));
			Assert.AreEqual(-1323428179, HashCodeUtility.CombineHashCodes(0, 0, 0, 0, 0, 1));
			Assert.AreEqual(1002546726, HashCodeUtility.CombineHashCodes(new[] { 0, 0, 0, 0, 0, 0 }));

			Assert.AreEqual(-1222849262, HashCodeUtility.CombineHashCodes(0, 0, 0, 0, 0, 0, 0));
			Assert.AreEqual(1531490892, HashCodeUtility.CombineHashCodes(0, 0, 0, 0, 0, 0, 1));
			Assert.AreEqual(-1222849262, HashCodeUtility.CombineHashCodes(new[] { 0, 0, 0, 0, 0, 0, 0 }));
		}

		[Test]
		public void CombineHashCodesDuplicate()
		{
			// test for bad hash code generation
			const string str = "happy";
			Assert.AreNotEqual(0, HashCodeUtility.CombineHashCodes(str.GetHashCode(), str.GetHashCode()));
			Assert.AreNotEqual(str.GetHashCode(), HashCodeUtility.CombineHashCodes(str.GetHashCode(), str.GetHashCode()));
		}

		[Test]
		public void CombineHashCodesNull()
		{
			HashCodeUtility.CombineHashCodes((int[]) null);
		}
	}
}
