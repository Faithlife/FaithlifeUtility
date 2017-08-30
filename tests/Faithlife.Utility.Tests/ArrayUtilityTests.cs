using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ArrayUtilityTests
	{
		[Test]
		public void CloneTest()
		{
			int[] anFirst = new int[] { 1, 2, 4, 8 };
			int[] anSecond = ArrayUtility.Clone(anFirst);
			Assert.AreEqual(anFirst.Length, anSecond.Length);
			Assert.AreNotSame(anFirst, anSecond);
		}

		[Test]
		public void ConcatenateOne()
		{
			int[] anFirst = new int[] { 1, 2, 4, 8 };
			Assert.AreEqual(0, ArrayUtility.Compare(ArrayUtility.Concatenate<int>(anFirst),
				new int[] { 1, 2, 4, 8 }));
		}

		[Test]
		public void ConcatenateThree()
		{
			int[] anFirst = new int[] { 1, 2, 4, 8 };
			int[] anSecond = new int[] { 8, 4, 2, 1 };
			int[] anThird = new int[] { 3, 5, 6, 7 };
			Assert.AreEqual(0, ArrayUtility.Compare(ArrayUtility.Concatenate(anFirst, anSecond, anThird),
				new int[] { 1, 2, 4, 8, 8, 4, 2, 1, 3, 5, 6, 7 }));
		}

		[Test]
		public void ConcatenateNone()
		{
			Assert.AreEqual(0, ArrayUtility.Compare(ArrayUtility.Concatenate<int>(), new int[] { }));
		}

		[Test]
		public void ConcatenateEmpty()
		{
			Assert.AreEqual(0, ArrayUtility.Compare(ArrayUtility.Concatenate(new int[] { }, new int[] { }), new int[] { }));
		}

		[Test]
		public void ConcatenateNullParams()
		{
			Assert.Throws<ArgumentNullException>(() => ArrayUtility.Concatenate<int>(null));
		}

		[Test]
		public void ConcatenateNullArray()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => ArrayUtility.Concatenate(new int[] { }, null));
		}

		[Test]
		public void ArraysEqual()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4, 5 };
			int[] anSecond = new int[] { 1, 2, 3, 4, 5 };
			Assert.IsTrue(ArrayUtility.AreEqual(anFirst, anSecond));
		}

		[Test]
		public void ArraysNotEqual()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4 };
			int[] anSecond = new int[] { 1, 2, 3, 5 };
			Assert.IsFalse(ArrayUtility.AreEqual(anFirst, anSecond));
		}

		[Test]
		public void ArraysEqualWithNull()
		{
			string[] anFirst = new string[] { "1", "2", null, "4", "5" };
			string[] anSecond = new string[] { "1", "2", null, "4", "5" };
			Assert.IsTrue(ArrayUtility.AreEqual(anFirst, anSecond));
		}

		[Test]
		public void NullArraysEqual()
		{
			Assert.IsTrue(ArrayUtility.AreEqual<int>(null, null));
		}

		[Test]
		public void ArrayNotEqualNullArray()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4 };
			Assert.IsFalse(ArrayUtility.AreEqual(anFirst, null));
		}

		[Test]
		public void EmptyArrayNotEqualNullArray()
		{
			int[] anFirst = new int[] {};
			Assert.IsFalse(ArrayUtility.AreEqual(anFirst, null));
		}

		[Test]
		public void TwoSizedArraysNotEqual()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4 };
			int[] anSecond = new int[] { 1, 2, 3 };
			Assert.IsFalse(ArrayUtility.AreEqual(anFirst, anSecond));
		}

		[Test]
		public void NullItemNotEqualNonNullItem()
		{
			string[] astrFirst = new string[] { "1", "2", null, "4" };
			string[] astrSecond  = new string[] { "1", "2", "3", "4" };
			Assert.IsFalse(ArrayUtility.AreEqual(astrFirst, astrSecond));
		}

		[Test]
		public void NonNullItemNotEqualNullItem()
		{
			string[] astrFirst = new string[] { "1", "2", "3", "4" };
			string[] astrSecond = new string[] { "1", "2", "3", null };
			Assert.IsFalse(ArrayUtility.AreEqual(astrFirst, astrSecond));
		}

		[Test]
		public void ArraysCompareEqual()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4, 5 };
			int[] anSecond = new int[] { 1, 2, 3, 4, 5 };
			Assert.AreEqual(0, ArrayUtility.Compare(anFirst, anSecond));
		}

		[Test]
		public void ArraysCompareEqualWithNull()
		{
			string[] astrFirst = new string[] { "1", "2", null, "4", "5" };
			string[] astrSecond = new string[] { "1", "2", null, "4", "5" };
			Assert.AreEqual(0, ArrayUtility.Compare(astrFirst, astrSecond));
		}

		[Test]
		public void NullArraysCompareEqual()
		{
			Assert.AreEqual(0, ArrayUtility.Compare<int>(null, null));
		}

		[Test]
		public void NullArrayLessThanArray()
		{
			int[] anSecond = new int[] { 1, 2, 3, 4 };
			Assert.Less(ArrayUtility.Compare(null, anSecond), 0);
		}

		[Test]
		public void ArrayGreaterThanNullArray()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4 };
			Assert.Greater(ArrayUtility.Compare(anFirst, null), 0);
		}

		[Test]
		public void ShorterArrayLessThanLongerArray()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4 };
			int[] anSecond = new int[] { 1, 2, 3, 4, 5 };
			Assert.Less(ArrayUtility.Compare(anFirst, anSecond), 0);
		}

		[Test]
		public void LongerArrayGreaterThanShorterArray()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4, 5 };
			int[] anSecond = new int[] { 1, 2, 3, 4 };
			Assert.Greater(ArrayUtility.Compare(anFirst, anSecond), 0);
		}

		[Test]
		public void NullItemLessThanNonNullItem()
		{
			string[] astrFirst = new string[] { "1", "2", "3", null };
			string[] astrSecond = new string[] { "1", "2", "3", "4" };
			Assert.Less(ArrayUtility.Compare(astrFirst, astrSecond), 0);
		}

		[Test]
		public void NonNullItemGreaterThanNullItem()
		{
			string[] astrFirst = new string[] { "1", "2", "3", "4" };
			string[] astrSecond = new string[] { "1", "2", "3", null };
			Assert.Greater(ArrayUtility.Compare(astrFirst, astrSecond), 0);
		}

		[Test]
		public void LesserItemLesserThanGreaterItem()
		{
			int[] anFirst = new int[] { 1, 2, 3, 4 };
			int[] anSecond = new int[] { 1, 2, 3, 5 };
			Assert.Less(ArrayUtility.Compare(anFirst, anSecond), 0);
		}

		[Test]
		public void GreaterItemGreaterThanLesserItem()
		{
			int[] anFirst = new int[] { 1, 2, 3, 5 };
			int[] anSecond = new int[] { 1, 2, 3, 4 };
			Assert.Greater(ArrayUtility.Compare(anFirst, anSecond), 0);
		}

		[Test]
		public void CreateTest()
		{
			var array = new[] { 1.0, 2.0, 3.0 };
			Assert.AreEqual(3, array.Length);
			Assert.AreEqual(typeof(double[]), array.GetType());
		}

		[Test]
		public void CreateEmptyTest()
		{
			var array = new double[0];
			Assert.AreEqual(0, array.Length);
		}
	}
}
