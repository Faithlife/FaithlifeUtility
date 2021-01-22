using System;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class ComparableImplTests
	{
		[Test]
		public void TestArgumentException()
		{
			var ti = new TestInt(1);
			object o = "test";
			Assert.Throws<ArgumentException>(() => ti.CompareTo(o));
		}

		[Test]
		public void TestLessThan()
		{
			var ti = new TestInt(1);
			object o = new TestInt(2);
			Assert.Less(ti.CompareTo(o), 0);
		}

		[Test]
		public void TestEqual()
		{
			var ti = new TestInt(1);
			object o = new TestInt(1);
			Assert.AreEqual(0, ti.CompareTo(o));
		}

		[Test]
		public void TestSame()
		{
			var ti = new TestInt(1);
			object o = ti;
			Assert.AreEqual(0, ti.CompareTo(o));
		}

		[Test]
		public void TestNull()
		{
			var ti = new TestInt(1);
			object? o = null;
			Assert.Greater(ti.CompareTo(o), 0);
		}

		[Test]
		public void OperatorLessThan()
		{
			TestInt? iNull = null;
			var i1 = new TestInt(1);
			var i1b = new TestInt(1);
			var i2 = new TestInt(2);

			Assert.IsTrue(ComparableImpl.OperatorLessThan(iNull, i1));
			Assert.IsFalse(ComparableImpl.OperatorLessThan(i1, iNull));
			Assert.IsFalse(ComparableImpl.OperatorLessThan(i1, i1));
			Assert.IsFalse(ComparableImpl.OperatorLessThan(i1, i1b));
			Assert.IsTrue(ComparableImpl.OperatorLessThan(i1, i2));
			Assert.IsFalse(ComparableImpl.OperatorLessThan(i2, i1));
		}

		[Test]
		public void OperatorLessThanOrEqual()
		{
			TestInt? iNull = null;
			var i1 = new TestInt(1);
			var i1b = new TestInt(1);
			var i2 = new TestInt(2);

			Assert.IsTrue(ComparableImpl.OperatorLessThanOrEqual(iNull, i1));
			Assert.IsFalse(ComparableImpl.OperatorLessThanOrEqual(i1, iNull));
			Assert.IsTrue(ComparableImpl.OperatorLessThanOrEqual(i1, i1));
			Assert.IsTrue(ComparableImpl.OperatorLessThanOrEqual(i1, i1b));
			Assert.IsTrue(ComparableImpl.OperatorLessThanOrEqual(i1, i2));
			Assert.IsFalse(ComparableImpl.OperatorLessThanOrEqual(i2, i1));
		}

		[Test]
		public void OperatorGreaterThan()
		{
			TestInt? iNull = null;
			var i1 = new TestInt(1);
			var i1b = new TestInt(1);
			var i2 = new TestInt(2);

			Assert.IsTrue(ComparableImpl.OperatorGreaterThan(i1, iNull));
			Assert.IsFalse(ComparableImpl.OperatorGreaterThan(iNull, i1));
			Assert.IsFalse(ComparableImpl.OperatorGreaterThan(i1, i1));
			Assert.IsFalse(ComparableImpl.OperatorGreaterThan(i1, i1b));
			Assert.IsFalse(ComparableImpl.OperatorGreaterThan(i1, i2));
			Assert.IsTrue(ComparableImpl.OperatorGreaterThan(i2, i1));
		}

		[Test]
		public void OperatorGreaterThanOrEqual()
		{
			TestInt? iNull = null;
			var i1 = new TestInt(1);
			var i1b = new TestInt(1);
			var i2 = new TestInt(2);

			Assert.IsTrue(ComparableImpl.OperatorGreaterThanOrEqual(i1, iNull));
			Assert.IsFalse(ComparableImpl.OperatorGreaterThanOrEqual(iNull, i1));
			Assert.IsTrue(ComparableImpl.OperatorGreaterThanOrEqual(i1, i1));
			Assert.IsTrue(ComparableImpl.OperatorGreaterThanOrEqual(i1, i1b));
			Assert.IsFalse(ComparableImpl.OperatorGreaterThanOrEqual(i1, i2));
			Assert.IsTrue(ComparableImpl.OperatorGreaterThanOrEqual(i2, i1));
		}

		private class TestInt : IComparable, IComparable<TestInt>
		{
			public TestInt(int nValue)
			{
				m_nValue = nValue;
			}

			public int CompareTo(object? obj)
			{
				return ComparableImpl.CompareToObject(this, obj);
			}

			public int CompareTo(TestInt? other)
			{
				if (other == null)
					return 1;
				return m_nValue.CompareTo(other.m_nValue);
			}

			private readonly int m_nValue;
		}
	}
}
